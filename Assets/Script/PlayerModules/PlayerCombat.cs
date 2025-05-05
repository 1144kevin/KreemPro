using Fusion;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    [SerializeField] private float attackCooldown = 0.5f;

    private bool attackLocked = false;
    private AttackHandler attackHandler;
    private AnimationHandler animationHandler;
    private NetworkObject netObj;

    private void Awake()
    {
        attackHandler = GetComponent<AttackHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        netObj = GetComponent<NetworkObject>();
    }

    public void HandleAttack(NetworkInputData input, NetworkButtons pressed, NetworkRunner runner)
    {
        if (!netObj.HasInputAuthority || attackLocked)
            return;

        if (pressed.IsSet((int)InputButton.ATTACK))
        {
            attackLocked = true;
            Invoke(nameof(UnlockAttack), attackCooldown);

            bool isRunning = input.direction.magnitude > 0.1f;

            if (netObj.HasStateAuthority)
                RpcPlayAttackAnimation(isRunning);

            if (netObj.HasInputAuthority)
                attackHandler?.Attack();
        }
        if (pressed.IsSet((int)InputButton.ATTACK))
        {
            Debug.Log("✅ 測試扣血：空白鍵被按下");

            if (netObj.HasStateAuthority)
            {
                var player = GetComponent<Player>();
                player?.TakeDamage(10); // 扣 10 點
            }
        }
    }


    private void UnlockAttack()
    {
        attackLocked = false;
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcPlayAttackAnimation(bool isRunning)
    {
        animationHandler?.TriggerAttack(isRunning);
    }
}
