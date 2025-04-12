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
        FindObjectOfType<RankingManager>().ShowRankingResults(); // 呼叫 UI 顯示
    }
}
