using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KreemTower : MonoBehaviour
{
    // 用於儲存每個玩家在 KreemTower 的吸收數量
    private Dictionary<GameObject, int>  RecordKreem = new Dictionary<GameObject, int>();

    public GameObject player;


    

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            StartCoroutine(HandlePlayerInteraction(collider.gameObject));
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // 停止檢查該玩家的停留狀態
            StopAllCoroutines();
        }
    }

    private IEnumerator HandlePlayerInteraction(GameObject other)
    {
        KreemCollect kreemCollect = other.GetComponent<KreemCollect>();
        // 停留時間計時
        float stayTime = 0f;

        while (stayTime < 3f)
        {
            // 如果玩家離開，則停止檢查
            yield return new WaitForSeconds(0.1f);
            stayTime += 0.1f;
            Debug.Log("1");
        }
        int KreemCount = kreemCollect.KreemCount;

        // 檢查該玩家是否有 KreemCount >= 1
        if (KreemCount >= 1)
        {
            // 確保玩家在字典中初始化
            if (! RecordKreem.ContainsKey(player))
            {
                 RecordKreem[player] = 0;
            }

            // 吸收玩家的 KreemCount
             RecordKreem[player] += KreemCount;
            Debug.Log($"Player {player.name}'s KreemCount Record: {RecordKreem[player]}");

            // 玩家身上的 KreemCount 歸零
            KreemCount = 0;

            // 檢查該玩家的吸收總數是否達到 3
            if (RecordKreem[player] >= 3)
            {
                // 啟動特效（需替換為實際特效觸發邏輯）
                Debug.Log($" special effect!");

                // 重置該玩家的吸收記錄
                RecordKreem[player] = 0;
            }
        }
    }
}
