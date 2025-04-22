using UnityEngine;
using Fusion;
using System.Collections;
public class EntrySceneController : MonoBehaviour
{
   private NetworkRunner runner;

    private float loadingTime = 2f;

    private void Start()
    {
        if (runner == null)
            runner = FindObjectOfType<NetworkRunner>();

        if (runner.IsServer)
        {
            Debug.Log("🧭 Host 開始模擬 loading，準備切換 FinalScene");
            StartCoroutine(DelayedLoadFinalScene());
        }
    }

    private IEnumerator DelayedLoadFinalScene()
    {
        yield return new WaitForSeconds(loadingTime);

        Debug.Log("🚀 Host 執行 LoadScene('FinalScene')");
        runner.LoadScene("FinalScene");
    }
}
