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
    direction.Normalize(); // 確保方向標準化
    float boosterMultiplier = booster?.GetSpeedMultiplier() ?? 1f;
    float actualSpeed = ((debugSpeedOverride > 0f) ? debugSpeedOverride : baseSpeed) * boosterMultiplier;

    // ✅ 核心修正：設定 controller 的 maxSpeed
    characterController.maxSpeed = actualSpeed;

    Vector3 moveDelta = actualSpeed * direction * runner.DeltaTime;
    characterController.Move(moveDelta);

    bool nowMoving = direction.magnitude > 0.1f;
    if (nowMoving != isMoving)
    {
        isMoving = nowMoving;

        // ✅ 本地立即動畫
        animationHandler?.PlayerAnimation(direction);

        // ✅ 廣播動畫給其他 Client
        if (Object.HasStateAuthority && player != null)
        {
            player.RpcUpdateAnimationState(direction);
        }
    }
}

}
