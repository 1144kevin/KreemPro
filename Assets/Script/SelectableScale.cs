using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableScale : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    private Vector3 originalScale;
    public float scaleMultiplier = 1.2f;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void OnSelect(BaseEventData eventData)
    {
        // ✅ 放大
        transform.localScale = originalScale * scaleMultiplier;

        // ✅ 移到最上層，避免被蓋住
        transform.SetAsLastSibling();
    }

    public void OnDeselect(BaseEventData eventData)
    {
        // 還原大小（不需要還原順序，這樣目前被選到的永遠最上層）
        transform.localScale = originalScale;
    }
}