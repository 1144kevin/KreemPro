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
        if (netObj == null || characterController == null || netObj.HasStateAuthority == false)
            return;

        // normalize direction
        Vector3 direction = input.direction;
        direction.Normalize();

        float boosterMultiplier = booster?.GetSpeedMultiplier() ?? 1f;
        float actualSpeed = ((debugSpeedOverride > 0f) ? debugSpeedOverride : baseSpeed) * boosterMultiplier;

        Vector3 moveDelta = actualSpeed * direction * runner.DeltaTime;
        characterController.maxSpeed = actualSpeed;
        characterController.Move(moveDelta);

        // 判斷是否切換動畫
        bool nowMoving = direction.magnitude > 0.1f;
        if (nowMoving != isMoving)
        {
            isMoving = nowMoving;
            RpcUpdateAnimationState(direction);
        }

        // InputAuthority 自己播放本地動畫
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
