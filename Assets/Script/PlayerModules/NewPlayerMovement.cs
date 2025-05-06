using Fusion;
using UnityEngine;

public class NewPlayerMovement : NetworkBehaviour
{
    [SerializeField] private float baseSpeed = 500f;
    [SerializeField] private float debugSpeedOverride = -1f;

    private Player player;
    private NetworkCharacterController characterController;
    private Booster booster;
    private AnimationHandler animationHandler;
    private bool isMoving = false;

    public override void Spawned()
    {
        characterController = GetComponent<NetworkCharacterController>();
        booster = GetComponent<Booster>();
        animationHandler = GetComponent<AnimationHandler>();
        player = GetComponent<Player>(); // ✅ 補上初始化，避免 null
    }

    public void HandleMovement(Vector3 direction, NetworkRunner runner)
    {
        float boosterMultiplier = booster?.GetSpeedMultiplier() ?? 1f;
        float actualSpeed = ((debugSpeedOverride > 0f) ? debugSpeedOverride : baseSpeed) * boosterMultiplier;
        Vector3 moveDelta = actualSpeed * direction * runner.DeltaTime;

        characterController.Move(moveDelta);

        bool nowMoving = direction.magnitude > 0.1f;
        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;

            // ✅ 自己立即播放動畫
            animationHandler?.PlayerAnimation(direction);

            // ✅ Host 廣播動畫更新（給其他 Client）
            if (Object.HasStateAuthority && player != null)
            {
                player.RpcUpdateAnimationState(direction);
            }
        }
    }
}
