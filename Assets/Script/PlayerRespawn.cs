using Fusion;
using UnityEngine;

public class PlayerRespawn : NetworkBehaviour
{
    [SerializeField] private ParticleSystem respawnEffect;

    private NetworkCharacterController characterController;
    public GameObject KreemPrefab;

    private void Awake()
    {
        characterController = GetComponent<NetworkCharacterController>();
        if (characterController == null)
        {
            Debug.LogError("NetworkCharacterController not found on Player!");
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcSetPlayerVisibility(bool isVisible)
    {
        Debug.Log($"[Respawn] RpcSetPlayerVisibility: Setting model visibility to {isVisible} for {Object.Id}");
        // 所有 Renderer
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            renderer.enabled = isVisible;
        }
        // 所有 Collider
        Collider[] colliders = GetComponentsInChildren<Collider>();
        foreach (Collider col in colliders)
        {
            col.enabled = isVisible;
        }
        // CharacterController組件
        var controller = GetComponent<NetworkCharacterController>();
        if (controller != null)
        {
            controller.enabled = isVisible;
        }
        Transform canvasTrans = transform.Find("HealthCanvas");
        if (canvasTrans != null)
        {
            canvasTrans.gameObject.SetActive(isVisible);
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void RpcPlayRespawnEffect()
    {
        if (respawnEffect != null)
        {
            respawnEffect.Stop();
            respawnEffect.Play();
        }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcKreemSpawn(Vector3 deathPos)
    {
        if (!Object.HasStateAuthority || KreemPrefab == null) return;

        Runner.Spawn(KreemPrefab, deathPos, Quaternion.identity, default(PlayerRef));
    }
    public void Respawn()
    {
        if (!Object.HasStateAuthority) return;
        Vector3 spawnPos = SpawnPosition.GetSpawnPosition(Object.InputAuthority);

        // 使用 Teleport 更新位置
        characterController.Teleport(spawnPos, Quaternion.identity);
        Debug.Log($"[Respawn] Respawning player {Object.Id} at spawn index {Object.InputAuthority}, position: {spawnPos}");

        // 更新玩家狀態：取得 Player 組件，並呼叫 SetHealthToMax() 來重置血量與顯示
        var player = GetComponent<Player>();
        if (player != null)
        {
            player.SetHealthToMax();
        }

        // 播放重生粒子特效
        RpcPlayRespawnEffect();
    }

    // 重生請求 RPC，由 InputAuthority 發送給 State Authority
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RpcRequestRespawn()
    {
        Respawn();
    }
}
