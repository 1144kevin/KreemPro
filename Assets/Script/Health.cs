using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;

    public GameObject gameObject;
    [SerializeField] private GameObject Kreem;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 确保血量不小于0
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 獲取玩家當前的位置和旋轉
        Vector3 playerPosition = transform.position;
        Quaternion playerRotation = transform.rotation;

        // 隱藏玩家物件
        gameObject.SetActive(false);

        // 在玩家位置生成 Kreem
        Instantiate(Kreem, playerPosition, playerRotation);
    }
}