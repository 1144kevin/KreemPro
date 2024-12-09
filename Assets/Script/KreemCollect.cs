using UnityEngine;

public class KreemCollect : MonoBehaviour
{
    public int KreemCount = 0;
    public int KreemMax = 3;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger == false && collision.gameObject.tag == "Player")
        {
            // 增加收集計數
            KreemCount++;
            // 打印收集的物件訊息（可選）
            Debug.Log($"Total Collected: {KreemCount}");
            if (KreemCount >= 3)
            {
                KreemCount = 3;
            }

            // 移除物件
            gameObject.SetActive(false);
        }
    }
}
