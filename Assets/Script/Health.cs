using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;
    [SerializeField] private GameObject Kreem;

    void Start()
    {
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.SetMaxHealth(maxHealth);
        }
    }

    // void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         TakeDamage(20);
    //     }
    // }

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

        Vector3 hiddenPosition = new Vector3(1000, -500, 1000); // 或者使用場景中一個隱藏的空物件

        // 將物件移到隱藏位置
        transform.position = hiddenPosition;

        // 在玩家位置生成 Kreem
        Instantiate(Kreem, playerPosition, playerRotation);
    }
}