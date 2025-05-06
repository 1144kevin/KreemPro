using Fusion;
using UnityEngine;

public class NewPlayerMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeed = 500f;
    [SerializeField] private float debugSpeedOverride = -1f;

    private NetworkCharacterController characterController;
    private Booster booster;
    private AnimationHandler animationHandler;
    private bool isMoving = false;
    private NetworkObject netObj;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        booster = GetComponent<Booster>();
        animationHandler = GetComponent<AnimationHandler>();
        netObj = GetComponent<NetworkObject>();
    }


public void HandleMove(NetworkInputData input, NetworkRunner runner)
{
    if (netObj == null || characterController == null)
        return;

    Vector3 direction = input.direction.normalized;
    float boosterMultiplier = booster?.GetSpeedMultiplier() ?? 1f;
    float actualSpeed = ((debugSpeedOverride > 0f) ? debugSpeedOverride : baseSpeed) * boosterMultiplier;
    Vector3 moveDelta = actualSpeed * direction * runner.DeltaTime;

    // ✅ Host 負責真實物理移動
    if (netObj.HasStateAuthority)
    {
        characterController.Move(moveDelta);
    }

    // ✅ Client 本地模擬畫面（不會同步給其他人）
    if (netObj.HasInputAuthority && !netObj.HasStateAuthority)
    {
        // 模擬移動使畫面不延遲
        transform.position += moveDelta;
    }

    // ✅ 所有人都更新動畫狀態（Host 負責 RPC 廣播）
    bool nowMoving = direction.magnitude > 0.1f;
    if (nowMoving != isMoving)
    {
        isMoving = nowMoving;
        RpcUpdateAnimationState(direction);
    }

    // ✅ Client 自己播放動畫（立即回饋）
    if (netObj.HasInputAuthority)
    {
        animationHandler?.PlayerAnimation(direction);
    }
}



    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateAnimationState(Vector3 input)
    {
        animationHandler?.PlayerAnimation(input);
    }
}
