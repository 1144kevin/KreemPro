// BarSection.cs
using UnityEngine;
using UnityEngine.UI;

public class BarSection : MonoBehaviour
{
    [Header("UI Components")]
    public Slider healthSlider;
    public Slider attackSlider;
    [SerializeField] private GameObject Kreem;

    
    [Header("Positioning")]
    public new Camera camera;
    public Transform target;
    public Vector3 offset;

    [Header("Health Settings")]
    [SerializeField] private float currentHealth = 100f;
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private float healthRecoveryRate = 5f;
    [SerializeField] private float healthRecoveryDelay = 3f;
    private float lastDamageTime;
    private bool isDead = false;
    
    [Header("Attack Settings")]
    [SerializeField] private float currentAttack = 100f;
    [SerializeField] private float maxAttack = 100f;
    [SerializeField] private float attackRecoveryRate = 10f;
    [SerializeField] private float attackRecoveryDelay = 1f;
    private float lastAttackTime;
    
    [Header("Damage Settings")]
    public float damageCooldown = 1f;
    private bool canTakeDamage = true;
    private float cooldownTimer = 0f;

    [Header("References")]
    [SerializeField] private GameObject playerObject;
    private DieRespawn dieRespawn;
    private Renderer[] playerRenderers;
    private bool kreemSpawned = false;

    void Start()
    {
        
        playerRenderers = playerObject.GetComponentsInChildren<Renderer>();
        InitializeSliders();
        UpdateBars();
        lastDamageTime = -healthRecoveryDelay;
        lastAttackTime = -attackRecoveryDelay;

        if (playerObject == null)
        {
            // playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject == null)
            {
                Debug.LogError("Player object not found! Make sure it has the 'Player' tag.");
                return;
            }
        }

        dieRespawn = playerObject.GetComponent<DieRespawn>();
        if (dieRespawn == null)
        {
            Debug.LogError("DieRespawn component not found on player!");
        }
    }

    private void InitializeSliders()
    {
        if (healthSlider == null)
        {
            Debug.LogError("Health Slider is not assigned to BarSection!");
        }
        else
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = 1f;
        }

        if (attackSlider == null)
        {
            Debug.LogError("Attack Slider is not assigned to BarSection!");
        }
        else
        {
            attackSlider.minValue = 0f;
            attackSlider.maxValue = 1f;
        }
    }

    private void UpdateBars()
    {
        UpdateHealthBar(currentHealth, maxHealth);
        UpdateAttackBar(currentAttack, maxAttack);
    }

    public void UpdateHealthBar(float current_val, float max_val)
    {
        if (healthSlider != null)
        {
            float healthPercentage = current_val / max_val;
            healthSlider.value = healthPercentage;
        }
    }

    public void UpdateAttackBar(float current_val, float max_val)
    {
        if (attackSlider != null)
        {
            float attackPercentage = current_val / max_val;
            attackSlider.value = attackPercentage;
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (!canTakeDamage || isDead) return;

        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        lastDamageTime = Time.time;
        UpdateHealthBar(currentHealth, maxHealth);
        
        canTakeDamage = false;
        cooldownTimer = damageCooldown;

        if (currentHealth <= 0 && !isDead)
        {
            OnPlayerDeath();
        }
    }

    public void ModifyAttack(float amount)
    {
        currentAttack = Mathf.Clamp(currentAttack + amount, 0, maxAttack);
        lastAttackTime = Time.time;
        UpdateAttackBar(currentAttack, maxAttack);
    }

    public void IncreaseHealth(float amount)
{
    if (isDead) return;
    
    currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
    UpdateHealthBar(currentHealth, maxHealth);
}

    public void ResetBarsToFull()
    {
        //Resetting health/attack bar to full...
        currentHealth = maxHealth;
        UpdateHealthBar(currentHealth, maxHealth);

        attackSlider.gameObject.SetActive(true);
        currentAttack = maxAttack;
        UpdateAttackBar(currentAttack, maxAttack);

        isDead = false;
        Debug.Log("Revive or not");
         foreach (Renderer renderer in playerRenderers)
        {
            renderer.enabled = true;
        }
        //attackSlider.enabled = false;
         kreemSpawned = false;
        Debug.Log($"Health reset to: {currentHealth}/{maxHealth}");
    }

    private void OnPlayerDeath()
    {
        Debug.Log("Player has died!");
        isDead = true;
    }

    public float GetCurrentHealth() => currentHealth;
    public float GetCurrentAttack() => currentAttack;

    void Update()
    {
        if (isDead)
        {
            attackSlider.gameObject.SetActive(false);
            // Hide all player renderers
            foreach (Renderer renderer in playerRenderers)
            {
                renderer.enabled = false;
            }
           if (!kreemSpawned)
            {
                GameObject spawnedKreem = Instantiate(Kreem, playerObject.transform.position, playerObject.transform.rotation);
                kreemSpawned = true;
            }

            if (Input.GetKeyDown(KeyCode.K) && dieRespawn != null)
            {
                dieRespawn.DestroyAndRespawn();
                ResetBarsToFull();
            }
            
        }

        // Handle damage cooldown
        if (!canTakeDamage)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canTakeDamage = true;
            }
        }

        // Handle health recovery (only if not dead)
        if (!isDead && Time.time >= lastDamageTime + healthRecoveryDelay && currentHealth < maxHealth)
        {
            currentHealth = Mathf.Min(maxHealth, currentHealth + (healthRecoveryRate * Time.deltaTime));
            UpdateHealthBar(currentHealth, maxHealth);
        }

        // Handle attack recovery
        if (Time.time >= lastAttackTime + attackRecoveryDelay && currentAttack < maxAttack)
        {
            currentAttack = Mathf.Min(maxAttack, currentAttack + (attackRecoveryRate * Time.deltaTime));
            UpdateAttackBar(currentAttack, maxAttack);
        }

        // Handle UI positioning
        if (camera != null && target != null)
        {
            transform.rotation = camera.transform.rotation;
            transform.position = target.position + offset;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MineEffect"))
        {
            Debug.Log("Player hit by mine!");
            TakeDamage(30);
        }
    }
}