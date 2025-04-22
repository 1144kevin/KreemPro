using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GameFlowManager : NetworkBehaviour
{
    [SerializeField] private float matchDuration = 180f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject winnerUI;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private string singleWinnerFormat = "Player {0} wins Kreem {1}";
    [SerializeField] private string tieWinnerFormat = "Players {0} tie with Kreem {1}";
    [SerializeField] private float waitBeforeRanking = 5f;

    [Networked] private float remainingTime { get; set; }
    private bool gameEnded = false;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            remainingTime = matchDuration;
    
        }
    }
    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || gameEnded) return;

        if (remainingTime > 0)
        {
            remainingTime -= Runner.DeltaTime;
            RpcUpdateTimer(remainingTime);
        }
        else
        {
            gameEnded = true;
            DecideWinner();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateTimer(float time)
    {
        if (timerText != null)
        {
            int min = Mathf.FloorToInt(time / 60f);
            int sec = Mathf.FloorToInt(time % 60f);
            timerText.text = $"{min:00}:{sec:00}";
        }
    }

    private void DecideWinner()
    {
        int maxKreem = -1;
        List<(Player player, PlayerRef playerRef)> topPlayers = new();
        // GameResultData.KreemCounts.Clear(); // 🔁 準備儲存分數給下一場景用
        GameResultData.KreemCounts.Clear();
        foreach (var playerRef in Runner.ActivePlayers)
        {
            var obj = Runner.GetPlayerObject(playerRef);
            if (obj == null) continue;

            Player p = obj.GetComponentInChildren<Player>();
            if (p == null) continue;

            p.RpcSetGameEnded();;

            GameResultData.KreemCounts[playerRef] = p.kreemCollect; // ✅ 儲存分數

            if (p.kreemCollect > maxKreem)
            {
                maxKreem = p.kreemCollect;
                topPlayers.Clear();
                topPlayers.Add((p, playerRef));
            }
            else if (p.kreemCollect == maxKreem)
            {
                topPlayers.Add((p, playerRef));
            }
        }

        if (topPlayers.Count == 0) return;

        RpcShowWinners(topPlayers.Select(p => p.playerRef).ToArray(), maxKreem);
        RpcDisableAllPlayerInput();
        StartCoroutine(LoadRankingSceneAfterDelay());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcShowWinners(PlayerRef[] winnerRefs, int score)
    {
        winnerUI.SetActive(true);

        if (winnerRefs.Length == 1)
        {
            string formatted = string.Format(singleWinnerFormat, winnerRefs[0].PlayerId, score);
            winnerText.text = formatted;
        }
        else
        {
            string names = string.Join(", ", winnerRefs.Select(r => $"Player {r.PlayerId}"));
            string formatted = string.Format(tieWinnerFormat, names, score);
            winnerText.text = formatted;
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcDisableAllPlayerInput()
    {
        foreach (var handler in FindObjectsOfType<InputHandler>())
        {
            handler.DisableInput();
        }

        Debug.Log("🛑 所有玩家輸入已禁用！");
    }

    private IEnumerator LoadRankingSceneAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeRanking);

        if (Object.HasStateAuthority)
        {
            Runner.LoadScene("RankingScene"); // ✅ 實際 RankingScene 名稱
        }
    }
}
