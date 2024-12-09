using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem; // 引入 Input System

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;
    [SerializeField] private GameObject Kreem;
    private DieRespawn dieRespawn;
    private bool isDead = false;
    private bool respawnInput = false; // 新增變數儲存復活按鍵輸入

    void Start()
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
            respawnInput = false; // 重置输入
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return; // 如果已經死亡，則不處理傷害
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // 確保血量不小於0
        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth);
        }

        // 檢查是否死亡
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
        currentHealth = maxHealth; // 確保復活時血量是滿的
        isDead = false; // 重置死亡狀態

        Debug.Log("Player has respawned with full health: " + currentHealth);
    }

    private void Die()
    {
        Debug.Log("Dieeeeeeee");
        // 獲取玩家當前的位置和旋轉
        Vector3 playerPosition = transform.position;
        Quaternion playerRotation = transform.rotation;

        Vector3 hiddenPosition = new Vector3(1000, -500, 1000); // 或者使用場景中一個隱藏的空物件
        // 將物件移到隱藏位置
        transform.position = hiddenPosition;

        // 在玩家位置生成 Kreem
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