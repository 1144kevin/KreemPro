using Fusion;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class RankingManager : NetworkBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private TMP_Text voteText;
    [SerializeField] private TMP_Text rankingText;
    [SerializeField] private Transform contentParent;
    [SerializeField] private GameObject rankingEntryPrefab;
    [SerializeField] private GameObject[] characterPrefabs;
    [SerializeField] private string finalSceneName = "FinalScene";

    private Dictionary<PlayerRef, bool> restartVotes = new();


    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(returnButton.gameObject);
    }
    public override void Spawned()
    {
        // 綁定按鈕事件（每個人都要）
        restartButton.onClick.AddListener(OnRestartClicked);
        returnButton.onClick.AddListener(OnReturnClicked);

        if (Object.HasStateAuthority)
        {
            // 建立投票表
            foreach (var player in Runner.ActivePlayers)
            {
                restartVotes[player] = false;
            }

            // Host 自己顯示 + 廣播資料給 client
            ShowRankingResults();
            RpcPopulateRankingUI();
            StartCoroutine(BroadcastKreemAfterDelay());
        }
        else
        {
            StartCoroutine(CheckDisconnected()); // ✅ Client 啟動斷線偵測
        }
    }
    private IEnumerator CheckDisconnected()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Runner == null || !Runner.IsRunning)
            {
                Debug.Log("🔌 Client 偵測 Host 離線，自動跳轉 Entry");
                SceneManager.LoadScene("Entry");
                yield break;
            }
        }
    }



    private IEnumerator BroadcastKreemAfterDelay()
    {
        yield return new WaitForSeconds(0.2f);

        var relay = FindObjectOfType<RankingNetworkRelay>();
        if (relay != null)
        {
            var players = GameResultData.KreemCounts.Keys.ToArray();
            var scores = GameResultData.KreemCounts.Values.ToArray();
            relay.RpcReceiveKreemData(players, scores);
            Debug.Log("✅ Host 廣播玩家分數成功");
        }
        else
        {
            Debug.LogError("❗找不到 RankingNetworkRelay，請確認場景中有該物件並加 NetworkObject");
        }
    }

    public void ShowRankingResults()
    {
        if (GameResultData.KreemCounts == null || GameResultData.KreemCounts.Count == 0)
        {
            rankingText.text = "No data available.";
            return;
        }

        int max = GameResultData.KreemCounts.Max(kv => kv.Value);

        var lines = GameResultData.KreemCounts
            .OrderByDescending(kv => kv.Value)
            .Select(kv =>
            {
                return $"Player {kv.Key.PlayerId} - {kv.Value} Kreem";
            });

        rankingText.text = string.Join("\n", lines);
    }

    public void PopulateLocalUI()
    {
        foreach (Transform child in contentParent)
            Destroy(child.gameObject);

        if (GameResultData.KreemCounts == null || GameResultData.KreemCounts.Count == 0)
            return;

        var ordered = GameResultData.KreemCounts
                     .OrderByDescending(kv => kv.Value)
                     .ToList();

        int highestScore = GameResultData.KreemCounts.Max(kv => kv.Value);

        foreach (var kv in ordered)
        {
            var playerRef = kv.Key;
            var score = kv.Value;

            if (!GameManager.Instance.PlayerList.TryGetValue(playerRef, out var pData))
                continue;

            string displayName = $"{pData.PlayerName} (Player {playerRef.PlayerId})";

            int idx = pData.SelectedCharacterIndex;
            GameObject prefab = (idx >= 0 && idx < characterPrefabs.Length)
                                ? characterPrefabs[idx]
                                : null;

            bool isWinner = score == highestScore; // 判斷是否為最高分                    

            var entryGO = Instantiate(rankingEntryPrefab, contentParent);
            entryGO.GetComponent<RankingEntryUI>()
                   .Setup(displayName, score, prefab,isWinner);
        }
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void RpcPopulateRankingUI()
    {
        PopulateLocalUI();
    }

    private void OnRestartClicked()
    {
        RpcVoteRestart(Runner.LocalPlayer);
        restartButton.interactable = false;
    }

    public void OnReturnClicked()
    {
        if (Object.HasStateAuthority)
        {
            Runner.Shutdown();
        }
        SceneManager.LoadScene("main menu"); // ✅ 實際主選單場景名稱
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RpcVoteRestart(PlayerRef player)
    {
        if (!Object.HasStateAuthority) return;
        if (!restartVotes.ContainsKey(player)) return;

        restartVotes[player] = true;
        int voted = restartVotes.Values.Count(v => v);
        RpcUpdateVoteText(voted, restartVotes.Count);

        if (restartVotes.Values.All(v => v))
        {
            Debug.Log("✅ 所有玩家同意 Restart，重新進入 FinalScene");
            Runner.LoadScene(finalSceneName);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateVoteText(int voted, int total)
    {
        if (voteText != null)
        {
            voteText.text = $"🗳 Restart 投票: {voted}/{total}";
        }
    }
}
