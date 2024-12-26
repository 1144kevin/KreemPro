// using UnityEngine;

// public class KreemCollect : MonoBehaviour
// {
//     public int KreemCount = 0;
//     public int KreemMax = 3;


//     private void OnCollisionEnter(Collision collision)
//     {
//         Debug.Log($"Total Collected");
//         if (collision.gameObject.tag == "Player")
//         {
//             // 增加收集計數
//             KreemCount++;
//             // 打印收集的物件訊息（可選）
//             Debug.Log($"Total Collected: {KreemCount}");
//             if (KreemCount >= 3)
//             {
//                 KreemCount = 3;
//             }

//             // 移除物件
//             gameObject.SetActive(false);
//         }
//     }
// }
using UnityEngine;

public class KreemCollect : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Robot") || collision.gameObject.CompareTag("Mushroom") || collision.gameObject.CompareTag("Leopard") || collision.gameObject.CompareTag("Eagle")) // 確保與玩家碰撞
        {
            // 嘗試獲取 PlayerInventory 組件
            PlayerInventory inventory = collision.gameObject.GetComponent<PlayerInventory>();
            if (inventory != null)
            {
                // 增加收集計數
                inventory.AddKreem();
                // 移除 Kreem 物件
                gameObject.SetActive(false);
            }
            else
            {
                Debug.LogWarning("PlayerInventory component not found on the player!");
            }
        }
    }
}

