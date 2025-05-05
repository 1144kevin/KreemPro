using Fusion;
using UnityEngine;

public class Booster : NetworkBehaviour
{
    [Header("Booster Settings")]
    public float refillTime = 5f;      // 再充能所需時間
    public float boostTime = 2f;       // 加速持續時間
    public float speedMultiplier = 2f; // 加速倍數

    [Networked] private float refillTimer { get; set; } = 0f;
    [Networked] private float boostTimer { get; set; } = 0f;
    [Networked] private bool isBoosting { get; set; } = false;
    [Networked] private bool isCharged { get; set; } = true;

    private Player player;

    public override void Spawned()
    {
        player = GetComponent<Player>();
    }

public override void FixedUpdateNetwork()
{
    if (!Object.HasStateAuthority || player == null) return;

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
        TryUseBoost();
    }
}


    public void TryUseBoost()
    {
        if (!isCharged || isBoosting) return;

         Debug.Log("✅ Boost ACTIVATED!");
        isBoosting = true;
        boostTimer = 0f;
        // 不在這裡設定 isCharged = false，因為 boost 結束後才應該進入 refill 階段
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
}
