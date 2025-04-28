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
    [SerializeField] private float detectRadius = 2.5f;
    [SerializeField] private LayerMask detectLayer;
    [SerializeField] private int spawnBatchSize = 10;
    [SerializeField] private float despawnDelay = 3f;

    private bool exploded = false;
    private readonly List<NetworkObject> spawnedPieces = new List<NetworkObject>();

private void FixedUpdate()
{
    if (!Runner.IsServer) return;
    if (exploded) return;

    // 防止BrokenPiecePrefabs或其他參數未設定
    if (brokenPiecePrefabs == null || brokenPiecePrefabs.Count == 0) return;
    if (detectLayer.value == 0) return;

    Collider[] hits = Physics.OverlapSphere(transform.position, detectRadius, detectLayer);

    foreach (var hit in hits)
    {
        // if (hit == null || hit.gameObject == null) continue;

        string tagSafe = "Untagged";
        try
        {
            tagSafe = hit.gameObject.tag;
        }
        catch
        {
            continue;
        }

        if (tagSafe == "Player" || tagSafe == "testBox")
        {
            exploded = true;
            StartCoroutine(Explode());
            break;
        }
    }
}


    private IEnumerator Explode()
    {
        // 分批Spawn碎片
        int count = 0;
        foreach (var prefab in brokenPiecePrefabs)
        {
            if (prefab == null) continue;

            var piece = Runner.Spawn(prefab, transform.position, Random.rotation);
            if (piece != null)
            {
                spawnedPieces.Add(piece);

                if (piece.TryGetComponent<Rigidbody>(out var rb))
                {
                    Vector3 randomDirection = Vector3.up + new Vector3(
                        Random.Range(-0.5f, 0.5f),
                        0,
                        Random.Range(-0.5f, 0.5f)
                    );
                    rb.AddForce(randomDirection.normalized * explosionForce, ForceMode.Impulse);
                    rb.AddTorque(Random.insideUnitSphere * explosionForce, ForceMode.Impulse);
                }
            }

            count++;
            if (count % spawnBatchSize == 0)
            {
                yield return null; // 分批等待
            }
        }

        // Spawn 特殊掉落物 (Kreem)
        if (specialItemPrefab.IsValid)
        {
            var item = Runner.Spawn(specialItemPrefab, transform.position + Vector3.up * 1.0f, Quaternion.identity);
            if (item.TryGetComponent<Rigidbody>(out var itemRb))
            {
                Vector3 launchDirection = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                itemRb.AddForce(launchDirection.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        // 自己消失
        Runner.Despawn(Object);

        // 碎片延遲自動消失
        StartCoroutine(DespawnSpawnedPieces());
    }

    private IEnumerator DespawnSpawnedPieces()
    {
        yield return new WaitForSeconds(despawnDelay);

        foreach (var piece in spawnedPieces)
        {
            if (piece != null && piece.IsValid)
            {
                Runner.Despawn(piece);
            }
        }

        spawnedPieces.Clear();
    }
}
