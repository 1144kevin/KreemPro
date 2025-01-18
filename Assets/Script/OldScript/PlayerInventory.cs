using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public int CurrentKreem = 0; // 當前擁有的 Kreem 數量
    public int KreemMax = 3;  // 玩家最多能擁有的 Kreem 數量
    public int TotalKreemCollected = 0; // 玩家總共獲得的 Kreem 數量

    public void AddKreem()
    {
        TotalKreemCollected++; // 總數量也增加
        // 如果當前擁有的數量未達最大值，才進行增加
        if (CurrentKreem < KreemMax)
        {
            CurrentKreem++; // 增加當前數量
            Debug.Log($"{gameObject.tag} collected a Kreem! Current: {CurrentKreem}, Total: {TotalKreemCollected}");
        }
        else
        {
            Debug.Log($"{gameObject.tag} already has the maximum Kreem ({KreemMax}).");
        }
    }
}
