using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGotHit : MonoBehaviour
{
    public HealthBar2 healthbar;  // Changed to public so we can assign it in inspector
    public float damageCooldown = 1f;
    private bool canTakeDamage = true;
    private float cooldownTimer = 0f;

    public void Start()
    {
        // Verify healthbar is assigned
        if (healthbar == null)
        {
            Debug.LogError("HealthBar2 component is not assigned to PlayerGotHit!");
        }
    }

    public void Update()
    {
        if (!canTakeDamage)
        {
            cooldownTimer -= Time.deltaTime;
            if (cooldownTimer <= 0)
            {
                canTakeDamage = true;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Collision detected with: {other.gameObject.tag}");  // Debug log
        
        if (other.gameObject.CompareTag("MineEffect") && canTakeDamage)
        {
            Debug.Log("Player hit by mine!");
            if (healthbar != null)
            {
                healthbar.TakeDamage(30);
                Debug.Log($"Current health after damage: {healthbar.GetCurrentHealth()}");  // Debug log
                
                // Start damage cooldown
                canTakeDamage = false;
                cooldownTimer = damageCooldown;
            }
        }
    }
}