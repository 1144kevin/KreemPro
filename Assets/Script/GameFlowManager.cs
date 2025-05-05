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
    [SerializeField] private float waitBeforeRanking = 5f;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;


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


    private HashSet<int> triggeredWarnings = new HashSet<int>();
    private bool isFinalCountdown = false;

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateTimer(float time)
    {
        if (timerText != null)
        {
            int min = Mathf.FloorToInt(time / 60f);
            int sec = Mathf.FloorToInt(time % 60f);
            timerText.text = $"{min:00}:{sec:00}";

            int intTime = Mathf.CeilToInt(time);

            if ((intTime == 61 || intTime == 31) && !triggeredWarnings.Contains(intTime))
            {
                triggeredWarnings.Add(intTime);
                TriggerTimerWarning(false); // 普通提醒
                sceneAudioSetter?.PlayTimeNotifySound(); // 播放提示音效
            }
            else if (intTime == 11 && !triggeredWarnings.Contains(11))
            {
                triggeredWarnings.Add(11);
                TriggerTimerWarning(false);
                sceneAudioSetter?.PlayTenSecCoundownSound();//播放倒數10秒音效
            }
            else if (intTime == 10 && !triggeredWarnings.Contains(10))
            {
                
                triggeredWarnings.Add(10);
                TriggerTimerWarning(true);
                
            }
        }
    }

    private void TriggerTimerWarning(bool finalCountdown)
    {
        if (timerText == null) return;

        Color originalColor = timerText.color;
        Vector3 originalScale = timerText.transform.localScale;

        if (finalCountdown)
        {
            isFinalCountdown = true;
            timerText.color = Color.red; // 進入最後倒數，直接變紅
        }
        else
        {
            timerText.color = Color.red; // 瞬間紅色
            LeanTween.scale(timerText.gameObject, originalScale * 2f, 1f).setEaseOutBack()
                .setOnComplete(() =>
                {
                    LeanTween.scale(timerText.gameObject, originalScale, 1f).setEaseInBack();
                    if (!isFinalCountdown)
                    {
                        timerText.color = originalColor; // 如果不是最後倒數，變回原本顏色
                    }
                });
        }
    }


    private void DecideWinner()
    {
        GameResultData.KreemCounts.Clear();

        foreach (var playerRef in Runner.ActivePlayers)
        {
            var obj = Runner.GetPlayerObject(playerRef);
            if (obj == null) continue;

            Player p = obj.GetComponentInChildren<Player>();
            if (p == null) continue;

            p.RpcSetGameEnded();
            GameResultData.KreemCounts[playerRef] = p.kreemCollect;
        }

        RpcEndGameUI();
        RpcDisableAllPlayerInput();
        StartCoroutine(LoadRankingSceneAfterDelay());
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcEndGameUI()
    {
        if (winnerUI == null) return;

        sceneAudioSetter?.PlayRingSound();
        winnerUI.SetActive(true);
        winnerUI.transform.localScale = Vector3.zero;

        // 彈出動畫
        LeanTween.scale(winnerUI, Vector3.one, 0.5f).setEaseOutBack()
            .setOnComplete(() =>
            {
                // 等 2 秒後，再縮回去
                LeanTween.delayedCall(2f, () =>
                {
                    LeanTween.scale(winnerUI, Vector3.zero, 0.5f).setEaseInBack();
                });
            });
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
            Runner.LoadScene("RankingScene");
        }
    }
}
