using UnityEngine;

public class KreemCollect : MonoBehaviour
{
    public GameObject Kreem;
    private float KreemCount = 0;

    private void OnTriggerEnter(Collider collider)
    {

        if (collider.tag == "Coin")
        {

            // 增加收集計數
            KreemCount++;

            // 打印收集的物件訊息（可選）
            Debug.Log($"Total Collected: {KreemCount}");

            // 移除物件
            collider.gameObject.SetActive(false);

        }

    }
}
