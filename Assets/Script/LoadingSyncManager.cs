using UnityEngine;
using Fusion;
using System.Linq;
using System.Collections;

public class LoadingSyncManager : NetworkBehaviour
{
    [SerializeField] private LoadingBar loadingBar;

    private int readyCount = 0;
    private int totalPlayers;


 public void SetLoadingBar(LoadingBar bar)
{
    loadingBar = bar;
}

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            totalPlayers = Runner.ActivePlayers.Count();
            Debug.Log($"🧩 玩家總數: {totalPlayers}");
        }

        loadingBar.OnLoadingComplete += NotifyReady;
        loadingBar.StartLoading();
    }

    private void NotifyReady()
    {
        Debug.Log("✅ 本地 Loading 完成，發送 RPC 通知 Host");
        RPC_ImReady();
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_ImReady()
    {
        readyCount++;
        Debug.Log($"📥 玩家完成 Loading: {readyCount}/{totalPlayers}");

        if (readyCount == totalPlayers)
        {
            Debug.Log("🚀 所有人都準備好了，Host 廣播切場景");
            RPC_ActivateScene();
        }
    }

[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
private void RPC_ActivateScene()
{
    Debug.Log("🟩 收到 RPC → ActivateScene");

    loadingBar.ActivateScene(); // 給 UI 顯示作用

    if (Object.HasStateAuthority)
    {
        Debug.Log("🎯 Host 等待 0.2 秒，確保 Client 準備好");
        StartCoroutine(DelaySceneLoad());
    }
}

private IEnumerator DelaySceneLoad()
{
    yield return new WaitForSeconds(0.2f);
    var runner = FindObjectOfType<NetworkRunner>();
    runner.LoadScene("FinalScene");
}

}
