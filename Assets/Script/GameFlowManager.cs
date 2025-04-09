using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class GameFlowManager : NetworkBehaviour
{
    public static GameFlowManager Instance { get; private set; }

    [SerializeField] private float matchDuration = 180f;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private GameObject winnerUI;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button returnButton;
    [SerializeField] private string singleWinnerFormat = "Player {0} wins Kreem {1}";
    [SerializeField] private string tieWinnerFormat = "Players {0} tie with Kreem {1}";

    private Dictionary<PlayerRef, bool> restartVotes = new();
    [Networked] private float remainingTime { get; set; }
    private bool gameEnded = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            remainingTime = matchDuration;
            foreach (var player in Runner.ActivePlayers)
            {
                restartVotes[player] = false;
            }
        }

        if (!Object.HasStateAuthority)
        {
            returnButton.gameObject.SetActive(false);
        }

        restartButton.onClick.AddListener(() =>
        {
            var localPlayer = FindObjectsOfType<Player>().FirstOrDefault(p => p.Object.HasInputAuthority);
            if (localPlayer != null)
            {
                localPlayer.RequestRestart();
                restartButton.interactable = false;
            }
        });

        returnButton.onClick.AddListener(() =>
        {
            if (Object.HasStateAuthority)
            {
                Runner.Shutdown();
                SceneManager.LoadScene("Entry"); // 你回選角頁面的場景名
            }
        });
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

        foreach (var playerRef in Runner.ActivePlayers)
        {
            var obj = Runner.GetPlayerObject(playerRef);
            if (obj == null) continue;

            Player p = obj.GetComponentInChildren<Player>();
            if (p == null) continue;

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

        restartButton.gameObject.SetActive(true);
        restartButton.interactable = true;

        if (Object.HasStateAuthority)
        {
            returnButton.gameObject.SetActive(true);
        }
    }

    public void RegisterRestartVote(PlayerRef player)
    {
        if (!restartVotes.ContainsKey(player)) return;

        restartVotes[player] = true;

        if (restartVotes.Values.All(v => v))
        {
            Runner.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
