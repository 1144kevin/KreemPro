// Attack.cs
using System.Collections;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public float maxAttack = 100;
    private float currentAttack;
    public AttackBar attackBar;
    private DieRespawn dieRespawn;
    private bool isDead = false;
    private bool respawnInput = false;

    [Header("Attack Recovery Settings")]
    [SerializeField] private float attackRecoveryRate = 20f;
    [SerializeField] private float attackRecoveryDelay = 0.1f;
    [SerializeField] private float attackTickRate = 0.1f;
    private float lastDamageTime;
    private float nextRecoveryTime;
    private float cooldownTimer = 0f;
    public float damageCooldown = 1f;

    private void Start()
    {
        currentAttack = maxAttack;
        if (attackBar != null)
        {
            attackBar.SetMaxAttack(maxAttack);
        }

        dieRespawn = GetComponent<DieRespawn>();
        if (dieRespawn == null)
        {
            Debug.LogError("DieRespawn component not found on the GameObject.");
        }

        lastDamageTime = -attackRecoveryDelay;
        nextRecoveryTime = Time.time;
        StartCoroutine(AttackRecoveryRoutine());
    }

    private IEnumerator AttackRecoveryRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(attackTickRate);

        while (true)
        {
            if (!isDead && Time.time >= lastDamageTime + attackRecoveryDelay && currentAttack < maxAttack)
            {
                float recoveryAmount = attackRecoveryRate * attackTickRate;
                currentAttack = Mathf.Min(maxAttack, currentAttack + Mathf.CeilToInt(recoveryAmount));
                attackBar.SetAttack(currentAttack);
            }
            yield return waitTime;
        }
    }

    public void AttackLost(float damage)
    {
        currentAttack -= damage;
        lastDamageTime = Time.time;
        currentAttack = Mathf.Clamp(currentAttack, 0, maxAttack);
        
        if (attackBar != null)
        {
            cooldownTimer = damageCooldown;
            attackBar.SetAttack(currentAttack);
        }
    }

    public bool CanFire()
    {
        return currentAttack > 0;
    }

    public void AttackBarsToFull()
    {
        if (attackBar != null)
        {
            attackBar.SetMaxAttack(maxAttack);
        }
        currentAttack = maxAttack;
        Debug.Log("Player has respawned with full attak: " + currentAttack);
    }
}