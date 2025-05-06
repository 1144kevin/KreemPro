using Fusion;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float healRate = 10f;
    [SerializeField] private float healDelay = 5f;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;
    [SerializeField] private ParticleSystem getHitEffect;
    [SerializeField] private ParticleSystem sharedHitEffect;
    [SerializeField] private PlayerRespawn playerRespawn;

    [Networked] public int Health { get; private set; }
    [Networked] public bool IsDead { get; private set; } = false;
    [Networked] private Vector3 LastDeathPosition { get; set; }

    private float lastDamageTime = 0f;
    private float healAccumulator = 0f;

    public void Init()
    {
        Health = maxHealth;
        if (Object.HasStateAuthority)
        {
            RpcUpdateHealth(Health);
        }
        else if (healthBar != null)
        {
            healthBar.SetHealth(Health);
        }
        if (playerRespawn == null)
            playerRespawn = GetComponent<PlayerRespawn>();
    }

    public void TickHeal(NetworkRunner runner)
    {
        if (!Object.HasStateAuthority || IsDead || Health >= maxHealth) return;

        if (Time.time - lastDamageTime >= healDelay)
        {
            healAccumulator += runner.DeltaTime * healRate;
            if (healAccumulator >= 1f)
            {
                int healAmount = Mathf.FloorToInt(healAccumulator);
                Health = Mathf.Clamp(Health + healAmount, 0, maxHealth);
                healAccumulator -= healAmount;
                RpcUpdateHealth(Health);
            }
        }
    }

    public void TakeDamage(int amount)
    {
        if (!Object.HasStateAuthority) return;

        Health -= amount;
        Health = Mathf.Clamp(Health, 0, maxHealth);

        lastDamageTime = Time.time;

        if (Health <= 0 && !IsDead)
        {
            HandleDeath(); // ✅ 改成呼叫內部簡化版
        }

        RpcUpdateHealth(Health);
        RpcPlayHitEffect();
        RpcPlaySharedHitEffect();
    }
    private void HandleDeath()
    {
        IsDead = true;
        LastDeathPosition = transform.position;

        if (playerRespawn == null)
            playerRespawn = GetComponent<PlayerRespawn>();

        playerRespawn?.RpcSetPlayerVisibility(false);
        RpcPlayDieSound();

        // ✅ 整合 Kreem 掉落
        if (Object.HasStateAuthority)
        {
            var player = GetComponent<Player>();
            var kreemCount = player.kreemCollect;
            int dropCount = Mathf.Max(kreemCount, 1);
            player.kreemCollect = 0;

            for (int i = 0; i < dropCount; i++)
            {
                Vector3 spawnPos = LastDeathPosition + Vector3.up * 150f;
                var kreem = Runner.Spawn(
                    player.kreemPrefab,
                    spawnPos,
                    Quaternion.identity,
                    Object.InputAuthority
                );

                if (kreem != null && kreem.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 randomDir = new Vector3(
                        Random.Range(-10f, 10f),
                        Random.Range(-50f, 0f),
                        Random.Range(-10f, 10f)
                    ).normalized;
                    rb.AddForce(randomDir * 2000f, ForceMode.Impulse);
                }
            }
        }

        if (Object.HasInputAuthority)
        {
            playerRespawn?.RpcSetPlayerVisibility(false); // ✅ 觸發 復活UI 模組
        }
    }



    public void Heal(int amount)
    {
        if (!Object.HasStateAuthority || IsDead) return;
        Health = Mathf.Clamp(Health + amount, 0, maxHealth);
        RpcUpdateHealth(Health);
    }




    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcUpdateHealth(int currentHealth)
    {
        if (healthBar != null)
            healthBar.SetHealth(currentHealth);
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RpcPlayDieSound()
    {
        sceneAudioSetter?.PlayDieSound();
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
    private void RpcPlayHitEffect()
    {
        var player = GetComponent<Player>();
        player?.RpcPlayHitEffect(); // ✅ 呼叫 Player.cs 裡那個會幫你清的版本
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcPlaySharedHitEffect()
    {
        var player = GetComponent<Player>();
        player?.RpcPlaySharedHitEffect(); // ✅ 同樣交給 Player 管
    }

    public void Revive()
    {
        if (!Object.HasStateAuthority) return;
        Health = maxHealth;
        IsDead = false;
        RpcUpdateHealth(Health);
        playerRespawn?.RpcSetPlayerVisibility(true);
    }
}
