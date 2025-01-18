using UnityEngine;
using TMPro; // 使用 TextMeshPro，如果你使用的是普通 Text，可以改成 UnityEngine.UI

public class PlayerUICounter : MonoBehaviour
{
    public TextMeshProUGUI kreemText; // UI 元素用於顯示 Kreem 數量
    private PlayerInventory playerInventory;

    private void Start()
    {
        if (playerInventory == null)
        {
            // 從同層級的物件中查找 PlayerInventory
            Transform parent = transform.parent;
            if (parent != null)
            {
                playerInventory = parent.GetComponentInChildren<PlayerInventory>();
            }
        }
    }

    private void Update()
    {
        if (playerInventory != null && kreemText != null)
        {

            // 更新 UI 文字顯示
            kreemText.text = $"Current {playerInventory.CurrentKreem}\n Total {playerInventory.TotalKreemCollected}";
        }
    }
}
