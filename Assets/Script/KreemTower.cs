using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KreemTower : MonoBehaviour
{
    // 用於儲存每個玩家在 KreemTower 的吸收數量
    private Dictionary<GameObject, int> RecordKreem = new Dictionary<GameObject, int>();
private Coroutine currentCoroutine;

private void OnTriggerEnter(Collider collider)
{
    if (currentCoroutine != null)
    {
        StopCoroutine(currentCoroutine); // 停止之前的協程
    }
    currentCoroutine = StartCoroutine(HandlePlayerInteraction(collider.gameObject));
}

private void OnTriggerExit(Collider collider)
{
    if (currentCoroutine != null)
    {
        StopCoroutine(currentCoroutine); // 停止協程
        currentCoroutine = null;
    }
}


    private IEnumerator HandlePlayerInteraction(GameObject player)
    {
        KreemCollect kreemCollect = player.GetComponent<KreemCollect>();
        if (kreemCollect == null)
        {
            Debug.LogWarning($"{player.name} 沒有kreem");
            yield break;
        }

        // 停留時間計時
        float stayTime = 1.0f;

        while (stayTime <= 3f)
        {
            yield return new WaitForSeconds(1.0f);
            
            if (stayTime == 1){
                Debug.Log("1");stayTime += 1.0f;
            }
            else if (stayTime == 2){
                Debug.Log("2");stayTime += 1.0f;
            }
            else{
                Debug.Log("3");break;
            }
        }

        int KreemCount = kreemCollect.KreemCount;

        // 檢查該玩家是否有 KreemCount >= 1
        if (KreemCount >= 1)
        {
            // 初始化該玩家的記錄
            if (!RecordKreem.ContainsKey(player))
            {
                RecordKreem[player] = 0;
            }

            // 吸收玩家的 KreemCount 並更新記錄
            RecordKreem[player] += KreemCount;
            Debug.Log($"{player.name}已蒐集{RecordKreem[player]}個kreem");

            // 將玩家的 KreemCount 歸零
            kreemCollect.KreemCount = 0;
            Debug.Log($"{player.name}的kreem已被歸零:{kreemCollect.KreemCount}");

            // 如果吸收總數 >= 3，觸發特效並重置記錄
            if (RecordKreem[player] >= 3)
            {
                Debug.Log($"{player.name}啟動kreem大招!");
                RecordKreem[player] = 0;
            }
        }
    }
}

