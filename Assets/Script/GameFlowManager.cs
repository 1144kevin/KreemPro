using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;
using System.Collections.Generic; // ✅ 解決 List<>
using System.Linq;               // ✅ 解決 Select()


public class GameFlowManager : NetworkBehaviour
{
    [SerializeField] private float matchDuration = 180f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject winnerUI;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private string singleWinnerFormat = "Player {0} wins Kreem {1}";
    [SerializeField]private string tieWinnerFormat = "Players {0} tie with Kreem {1}";

    

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
        Debug.Log("開始判定勝利者...");

        int maxKreem = -1;
        List<(Player player, PlayerRef playerRef)> topPlayers = new();

        foreach (var playerRef in Runner.ActivePlayers)
        {
            var obj = Runner.GetPlayerObject(playerRef);

            if (obj == null)
            {
                Debug.LogWarning($"找不到玩家物件：{playerRef}");
                continue;
            }

            Player p = obj.GetComponentInChildren<Player>();
            if (p == null)
            {
                Debug.LogWarning($"找不到 Player 腳本於 {obj.name} 或子物件中");
                continue;
            }

            Debug.Log($"✅ 玩家 {playerRef.PlayerId} 擁有 Kreem：{p.kreemCollect}");

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

        if (topPlayers.Count == 0)
        {
            Debug.LogWarning("❗未找到勝利者！");
            return;
        }

        // 呼叫 UI 顯示多人或單人勝利
        RpcShowWinners(topPlayers.Select(p => p.playerRef).ToArray(), maxKreem);
        RpcDisableAllInputs(); // ✅ 所有 client 都會執行 DisableInput()

        StartCoroutine(LoadSummaryScene());

    }


    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcShowWinners(PlayerRef[] winnerRefs, int score)
    {
        if (winnerUI != null)
        {
            winnerUI.SetActive(true);

            if (winnerRefs.Length == 1)
            {
                // 單人勝利格式
                string formatted = string.Format(singleWinnerFormat, winnerRefs[0].PlayerId, score);
                winnerText.text = formatted;
            }
            else
            {
                // 多人平手格式，手動換行！
                string names = string.Join(", ", winnerRefs.Select(r => $"Player {r.PlayerId}"));
                string formatted = string.Format(tieWinnerFormat, names, score);
                winnerText.text = formatted;
            }
        }
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcDisableAllInputs()
    {
        var inputHandlers = FindObjectsOfType<InputHandler>();
        foreach (var handler in inputHandlers)
        {
            handler.DisableInput(); // 每位本地玩家停用輸入
        }
    }
    private IEnumerator LoadSummaryScene()
    {
        yield return new WaitForSeconds(5f);
        if (Object.HasStateAuthority)
        {
            Runner.LoadScene("SummaryScene");
        }
    }
}
