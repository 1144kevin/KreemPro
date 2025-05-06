using Fusion;
using UnityEngine;

public class NewPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float baseSpeed = 500f;
    [SerializeField] private float debugSpeedOverride = -1f;

    private NetworkCharacterController characterController;
    private Booster booster;
    private AnimationHandler animationHandler;
    private bool isMoving = false;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();
        booster = GetComponent<Booster>();
        animationHandler = GetComponent<AnimationHandler>();
    }

    public override void FixedUpdateNetwork()
    {
        var input = Runner.GetInputForPlayer<NetworkInputData>(Object.InputAuthority);
        if (input == null)
            return;

        Vector3 direction = input.Value.direction.normalized;
        float boosterMultiplier = booster?.GetSpeedMultiplier() ?? 1f;
        float actualSpeed = ((debugSpeedOverride > 0f) ? debugSpeedOverride : baseSpeed) * boosterMultiplier;
        Vector3 moveDelta = actualSpeed * direction * Runner.DeltaTime;

        // ✅ 不分 Host/Client，自己動自己
        characterController.Move(moveDelta);

        bool nowMoving = direction.magnitude > 0.1f;
        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            // ✅ 自己播動畫（讓本地立即看到）
            animationHandler?.PlayerAnimation(direction);

            // ✅ 透過 Player.cs 的 RPC 廣播動畫狀態
            var player = GetComponent<Player>();
            if (player != null && Object.HasStateAuthority)
            {
                player.RpcUpdateAnimationState(direction);
            }
        }
    }


}
