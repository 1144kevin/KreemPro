using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar2 : MonoBehaviour
{
    public Slider slider;
    public new Camera camera;
    public Transform target;
    public Vector3 offset;
    
    private float currentHealth = 100f;
    private float maxHealth = 100f;

    void Start()
    {
        // Check if slider is assigned
        if (slider == null)
        {
            Debug.LogError("Slider is not assigned to HealthBar2!");
            return;
        }
        
        // Set the slider's min and max values
        slider.minValue = 0f;
        slider.maxValue = 1f;
        
        // Initialize health bar
        Debug.Log($"Initializing health bar: Current Health = {currentHealth}, Max Health = {maxHealth}");
        UpdateHealthBar(currentHealth, maxHealth);
    }

    public void UpdateHealthBar(float current_val, float max_val)
    {
        if (slider != null)
        {
            float healthPercentage = current_val / max_val;
            slider.value = healthPercentage;
            Debug.Log($"Health bar updated: Current Value = {current_val}, Max Value = {max_val}, Percentage = {healthPercentage}");
        }
    }

    public void TakeDamage(float damageAmount)
    {
        currentHealth = Mathf.Max(0, currentHealth - damageAmount);
        Debug.Log($"Taking damage: {damageAmount}. New health: {currentHealth}");
        UpdateHealthBar(currentHealth, maxHealth);
        
        if (currentHealth <= 0)
        {
            Debug.Log("Player has died!");
        }
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    // public void Heal(float healAmount)
    // {
    //     currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
    //     Debug.Log($"Healing: {healAmount}. New health: {currentHealth}");
    //     UpdateHealthBar(currentHealth, maxHealth);
    // }

    void Update()
    {
        if (camera != null && target != null)
        {
            transform.rotation = camera.transform.rotation;
            transform.position = target.position + offset;
        }
    }
}