using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TreasureBox : NetworkBehaviour
{
    [Header("爆破設定")]
    [SerializeField] public List<NetworkObject> brokenPiecePrefabs = new List<NetworkObject>();
    [SerializeField] private NetworkPrefabRef specialItemPrefab;
    [SerializeField] private float explosionForce = 10f;
    [SerializeField] private int spawnBatchSize = 10;
    [SerializeField] private float respawnDelay = 5f;

    private bool exploded = false;
    private Vector3 spawnPosition;
    [SerializeField] private NetworkPrefabRef selfPrefabRef;

    public override void Spawned()
    {
        if (!Runner || !Runner.IsServer) return;
        spawnPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!Runner || !Runner.IsServer || exploded) return;

        if (other.CompareTag("Player") || other.CompareTag("testBox"))
        {
            Debug.Log($"[TreasureBox] 玩家觸發爆破：{other.name}");
            exploded = true;
            StartCoroutine(DelayExplode(0.3f));
        }
    }

    private IEnumerator DelayExplode(float delay)
    {
        yield return new WaitForSeconds(delay);
        StartCoroutine(Explode());
    }

    private IEnumerator Explode()
    {
        int count = 0;
        Vector3 boxPosition = transform.position;

        SetTreasureBoxActive(false); // ✅ 隱藏自己

        foreach (var prefab in brokenPiecePrefabs)
        {
            if (prefab == null) continue;

            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(0.05f, 0.15f),
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 finalPosition = boxPosition + spawnOffset;
            var piece = Runner.Spawn(prefab, finalPosition, Random.rotation);

            if (piece != null && piece.TryGetComponent<Collider>(out var col))
            {
                col.isTrigger = true;
            }

            count++;
            if (count % spawnBatchSize == 0)
                yield return null;
        }

        if (specialItemPrefab.IsValid)
        {
            Vector3 itemPosition = boxPosition + Vector3.up;
            var item = Runner.Spawn(specialItemPrefab, itemPosition, Quaternion.identity, inputAuthority: null);

            if (item != null && item.TryGetComponent<Rigidbody>(out var itemRb))
            {
                Vector3 launchDir = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                itemRb.AddForce(launchDir.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        Debug.Log($"[TreasureBox] 爆炸完成，{respawnDelay} 秒後重新出現");
        StartCoroutine(ReappearAfterDelay(respawnDelay));
    }

    private IEnumerator ReappearAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SetTreasureBoxActive(true);
        exploded = false;
        Debug.Log($"[TreasureBox] ✅ 已重新啟用");
    }

    private void SetTreasureBoxActive(bool active)
    {
        // ✅ 控制所有 MeshRenderer
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            renderer.enabled = active;
        }

        // ✅ 控制所有 Collider
        foreach (var col in GetComponentsInChildren<Collider>())
        {
            col.enabled = active;
        }

        // ✅ 若有動畫/音效/特效等可加上 Enable 處理
    }
}
