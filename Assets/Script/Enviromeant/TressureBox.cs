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
            StartCoroutine(Explode());
        }
    }

    private IEnumerator Explode()
    {
        int count = 0;

        foreach (var prefab in brokenPiecePrefabs)
        {
            if (prefab == null) continue;

            var piece = Runner.Spawn(prefab, transform.position, Random.rotation, inputAuthority: null);
            if (piece != null)
            {
                Debug.Log($"[TreasureBox] Spawn 成功：{piece.name}, IsValid: {piece.IsValid}");

                if (piece.TryGetComponent<Rigidbody>(out var rb))
                {
                    var col = piece.GetComponent<Collider>();
                    if (col != null) col.isTrigger = true;

                    Vector3 randomDir = Random.onUnitSphere;
                    randomDir.y = Mathf.Abs(randomDir.y);

                    rb.AddForce(randomDir * explosionForce * 2f, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * explosionForce * 2f, ForceMode.Impulse);
                }
            }
            else
            {
                Debug.LogWarning($"[TreasureBox] Spawn 碎片失敗：{prefab.name}");
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
