using System.Linq;
using Fusion;
using UnityEngine;

public class RankingNetworkRelay : NetworkBehaviour
{
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcReceiveKreemData(PlayerRef[] players, int[] scores)
    {
        if (Runner.IsServer) return;

        GameResultData.KreemCounts.Clear();
        for (int i = 0; i < players.Length; i++)
        {
            GameResultData.KreemCounts[players[i]] = scores[i];
        }

        Debug.Log("✅ Client 收到所有玩家分數");

        var mgr = FindObjectOfType<RankingManager>();
        if (mgr != null)
        {
            mgr.ShowRankingResults();  // 更新文字
            mgr.PopulateLocalUI();     // 直接本地畫，不再叫 RPC
        }
    }
}
