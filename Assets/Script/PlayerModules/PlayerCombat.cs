using Fusion;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private float attackCooldown = 0.5f;

    private bool attackLocked = false;
    private AttackHandler attackHandler;
    private AnimationHandler animationHandler;
    private NetworkObject netObj;
    private PlayerHealth playerHealth;

    private void Awake()
    {
        attackHandler = GetComponent<AttackHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        netObj = GetComponent<NetworkObject>();
        playerHealth = GetComponent<PlayerHealth>();
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

            if (netObj.HasInputAuthority)
            {
                var player = GetComponent<Player>();
                // 播音效
                // player?.playerAudio?.PlayAttackSound(player.characterSoundIndex);

                animationHandler?.TriggerAttack(isRunning); // ✅ 自己立即看到

                // ✅ 請求 Host 廣播動畫（這樣 Host 與其他 Client 才會看到）
                RpcRequestPlayAttackAnimation(isRunning);
            }

            // ✅ Host 廣播動畫給所有 Client
            if (netObj.HasStateAuthority)
            {
                RpcPlayAttackAnimation(isRunning);
            }

        }
    }
    public void TickCombat(NetworkInputData input)
    {
        if (input.damageTrigger && playerHealth != null && !playerHealth.IsDead)
        {
            playerHealth.TakeDamage(10);
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
    
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcRequestPlayAttackAnimation(bool isRunning)
    {
        // Host 收到請求 → 廣播給所有人（包括自己與其他 Client）
        RpcPlayAttackAnimation(isRunning);
    }
}
