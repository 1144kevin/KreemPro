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

    private bool exploded = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!Runner || !Runner.IsServer || exploded) return;

        if (other.CompareTag("Player") || other.CompareTag("testBox"))
        {
            Debug.Log($"[TreasureBox] 玩家觸發爆破：{other.name}");
            exploded = true;
            StartCoroutine(DelayExplode(0.3f)); // 延遲避免 Spawn 在 (0,0,0)
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

        foreach (var prefab in brokenPiecePrefabs)
        {
            if (prefab == null) continue;

            // 加入隨機位移，避免碎片重疊生成
            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(0f, 0.2f),
                Random.Range(-0.3f, 0.3f)
            );

            var piece = Runner.Spawn(prefab, transform.position + spawnOffset, Random.rotation, inputAuthority: null);
            if (piece != null && piece.TryGetComponent<Rigidbody>(out var rb))
            {
                Debug.Log($"[TreasureBox] Spawn 成功：{piece.name}, IsValid: {piece.IsValid}");

                var col = piece.GetComponent<Collider>();
                if (col != null) col.isTrigger = false;

                // 碎片物理設定
                rb.mass = 1.5f;
                rb.drag = 0.1f;
                rb.angularDrag = 0.2f;
                rb.useGravity = true;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
                rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

                // 四散方向與推力
                Vector3 randomDir = Random.insideUnitSphere;
                randomDir.y = Mathf.Abs(randomDir.y * 0.5f) + 0.3f;
                randomDir.x *= 1.5f;
                randomDir.z *= 1.5f;

                float finalForce = explosionForce * 2.0f;
                rb.AddForce(randomDir * finalForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * explosionForce * 1.5f, ForceMode.Impulse);
            }
            else
            {
                Debug.LogWarning($"[TreasureBox] Spawn 碎片失敗：{prefab?.name}");
            }

            count++;
            if (count % spawnBatchSize == 0)
                yield return null;
        }

        // 特殊掉落物
        if (specialItemPrefab.IsValid)
        {
            var item = Runner.Spawn(specialItemPrefab, transform.position + Vector3.up, Quaternion.identity, inputAuthority: null);
            if (item.TryGetComponent<Rigidbody>(out var itemRb))
            {
                Vector3 launchDir = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                itemRb.AddForce(launchDir.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        Runner.Despawn(Object);
    }
}
