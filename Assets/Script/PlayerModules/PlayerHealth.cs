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

    public void TakeDamage(int damage)
    {
        if (!Object.HasStateAuthority || IsDead) return;

        Health -= damage;
        Health = Mathf.Clamp(Health, 0, maxHealth);
        lastDamageTime = Time.time;

        RpcUpdateHealth(Health);
        RpcPlayHitEffect();
        RpcPlaySharedHitEffect();
    }
    public void Heal(int amount)
    {
        if (!Object.HasStateAuthority || IsDead) return;
        Health = Mathf.Clamp(Health + amount, 0, maxHealth);
        RpcUpdateHealth(Health);
    }

    public void CheckDeathDrop(NetworkRunner runner, Transform playerTransform, ref int kreemCount, NetworkPrefabRef kreemPrefab)
    {
        if (!Object.HasStateAuthority || IsDead || Health > 0) return;

        IsDead = true;
        LastDeathPosition = playerTransform.position;
        playerRespawn.RpcSetPlayerVisibility(false);
        RpcPlayDieSound();

        int dropCount = Mathf.Max(kreemCount, 1);
        kreemCount = 0;

        for (int i = 0; i < dropCount; i++)
        {
            Vector3 spawnPos = LastDeathPosition + Vector3.up * 150f;
            var kreem = runner.Spawn(kreemPrefab, spawnPos, Quaternion.identity, Object.InputAuthority);

            if (kreem.TryGetComponent<Rigidbody>(out var rb))
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
        if (getHitEffect != null)
        {
            if (!getHitEffect.gameObject.activeSelf)
                getHitEffect.gameObject.SetActive(true);

            getHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            getHitEffect.Play();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcPlaySharedHitEffect()
    {
        if (sharedHitEffect != null)
        {
            sharedHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            sharedHitEffect.Play();
        }
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
