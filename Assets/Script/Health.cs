using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// public class Health : MonoBehaviour
// {
//     public int maxHealth=100;
//     private int currentHealth;
//     public HealthBar healthBar;

//     void Start (){
//         currentHealth = maxHealth;
//         healthBar.SetMaxHealth(maxHealth);
//     }
//     void Update () {
//         if(Input.GetKeyDown(KeyCode.Space)){
//             TakeDamage(20);
//         }
//     }
//     void TakeDamage(int damage){
//         currentHealth-=damage;
//         healthBar.SetHealth(currentHealth);
//     }


// }
public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public HealthBar healthBar;

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
        // Debug.Log($"{gameObject.name} has died!");
        // 在这里可以处理玩家死亡的逻辑，比如重置关卡或播放动画。
    }
}