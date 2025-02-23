using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;
    [SerializeField] private GameObject Kreem;
    private DieRespawn dieRespawn;
    private bool isDead = false;
    private bool respawnInput = false;

    [Header("Health Recovery Settings")]
    [SerializeField] private float healthRecoveryRate = 10f;  // Health recovered per second
    [SerializeField] private float healthRecoveryDelay = 3f;  // Delay before recovery starts after taking damage
    [SerializeField] private float recoveryTickRate = 0.1f;   // How often recovery occurs (in seconds)
    private float lastDamageTime;
    private float nextRecoveryTime;
    private float cooldownTimer = 0f;
    public float damageCooldown = 1f;

    private void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }

        dieRespawn = GetComponent<DieRespawn>();
        if (dieRespawn == null)
        {
            Debug.LogError("DieRespawn component not found on the GameObject.");
        }

        lastDamageTime = -healthRecoveryDelay;
        nextRecoveryTime = Time.time;
        StartCoroutine(HealthRecoveryRoutine());
    }

    private IEnumerator HealthRecoveryRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(recoveryTickRate);

        while (true)
        {
            if (!isDead && Time.time >= lastDamageTime + healthRecoveryDelay && currentHealth < maxHealth)
            {
                float recoveryAmount = healthRecoveryRate * recoveryTickRate;
                currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.CeilToInt(recoveryAmount));
                healthBar.SetHealth(currentHealth);
            }
            yield return waitTime;
        }
    }

    public void SetRespawnInput(bool input)
    {
        respawnInput = input;
    }

    void Update()
    {
        if (isDead && respawnInput)
        {
            Debug.Log("Player is pressing K to respawn.");
            dieRespawn.DestroyAndRespawn();
            ResetBarsToFull();
            isDead = false;
            respawnInput = false;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        lastDamageTime = Time.time;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if (healthBar != null)
        {
            cooldownTimer = damageCooldown;
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Die();
        }
    }

    public void ResetBarsToFull()
    {
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
        currentHealth = maxHealth;
        isDead = false;
        Debug.Log("Player has respawned with full health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Dieeeeeeee");
        Vector3 playerPosition = transform.position;
        Quaternion playerRotation = transform.rotation;
        Vector3 hiddenPosition = new Vector3(1000, -500, 1000);
        transform.position = hiddenPosition;

        if (Kreem != null)
        {
            Instantiate(Kreem, playerPosition, playerRotation);
        }
        else
        {
            Debug.LogError("Kreem GameObject is not assigned!");
        }
    }
}