using Fusion;
using UnityEngine;

public class Booster : NetworkBehaviour
{
    [Header("Booster Settings")]
    public float refillTime = 5f;
    public float boostTime = 2f;
    public float speedMultiplier = 2f;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;

    [Networked] private float refillTimer { get; set; } = 0f;
    [Networked] private float boostTimer { get; set; } = 0f;
    [Networked] private bool isBoosting { get; set; } = false;
    [Networked] private bool isCharged { get; set; } = false; // 一開始是空的

    [Networked] private float startDelayTimer { get; set; } = 0f;
    [Networked] private bool canStartRefill { get; set; } = false;
    public float startDelay = 5f;

    private Player player;
    private BoosterUI boosterUI; // 記錄 UI 的參考

    public float refillTimerPublic => refillTimer;
    public float refillTimePublic => refillTime;

    public GameObject playerUIPrefab;

    public override void Spawned()
    {
        player = GetComponent<Player>();

        if (Object.HasInputAuthority)
        {
            GameObject uiInstance = Instantiate(playerUIPrefab);
            BoosterUI ui = FindObjectOfType<BoosterUI>();
            if (ui != null)
            {
                ui.booster = this;
                boosterUI = ui; // 儲存 UI 參考
            }
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (!Object.HasStateAuthority || player == null) return;

        if (!canStartRefill)
        {
            startDelayTimer += Runner.DeltaTime;
            if (startDelayTimer >= startDelay)
            {
                canStartRefill = true;
            }
            return;
        }

        if (!isCharged && !isBoosting)
        {
            refillTimer += Runner.DeltaTime;
            if (refillTimer >= refillTime)
            {
                isCharged = true;
                refillTimer = 0f;
            }
        }

        if (isBoosting)
        {
            boostTimer += Runner.DeltaTime;
            if (boostTimer >= boostTime)
            {
                isBoosting = false;
                boostTimer = 0f;
                isCharged = false;
            }
        }

        if (GetInput(out NetworkInputData data) && data.boostTrigger)
        {
            bool boostActivated = TryUseBoost();

            // ✅ 只有在真的啟動加速、而且是自己這台機器，才播放音效
            if (boostActivated )
            {
                Rpc_PlaySpeedUpSound();
            }
        }
    }


    public bool TryUseBoost()
    {
        if (!isCharged || isBoosting) return false;

        Debug.Log("✅ Boost ACTIVATED!");
        isBoosting = true;
        boostTimer = 0f;
        isCharged = false;

        return true; // ✅ 成功啟動加速
    }

    public float GetSpeedMultiplier()
    {
        return isBoosting ? speedMultiplier : 1f;
    }

    public bool IsBoosting()
    {
        return isBoosting;
    }

    public bool IsCharged()
    {
        return isCharged;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void Rpc_PlaySpeedUpSound()
    {
        sceneAudioSetter?.PlaySpeedupSound();
    }
}

