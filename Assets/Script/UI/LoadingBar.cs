using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    public System.Action OnLoadingComplete;
    

    public void StartLoading()
    {
        StartCoroutine(FakeProgress());
    }

    IEnumerator FakeProgress()
    {
        float progress = 0f;
        while (progress < 1f)
        {
            progress += Time.deltaTime * 0.5f;
            fillImage.fillAmount = progress;
            yield return null;
        }

        Debug.Log("✅ 模擬 Loading 完成，通知 LoadingSyncManager");
        OnLoadingComplete?.Invoke();
    }

    // 這裡不再處理場景跳轉
    public void ActivateScene()
    {
        Debug.Log("🟩 UI 收到廣播，Loading 結束");
        // 不執行 LoadScene，交由 LoadingSyncManager 處理
    }
}
