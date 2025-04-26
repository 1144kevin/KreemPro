using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TreasureBox : NetworkBehaviour
{
    [Header("Prefabs")]
    public NetworkPrefabRef BrokenBoxPrefab;
    public NetworkPrefabRef SpecialItemPrefab;

    [Header("Explosion Settings")]
    public float coinForce = 10f;
    public float brokenUpwardForce = 5f;

    [Header("Broken Box Settings")]
    [SerializeField] private float brokenPieceDespawnTime = 3f; // 🛠️ 子物件碎片消失時間

    private bool exploded = false;

    private readonly List<NetworkObject> spawnedPieces = new List<NetworkObject>(); // 記錄生成的碎片們

    private void OnTriggerEnter(Collider other)
    {
        if (!Runner.IsServer) return;
        if (exploded) return;
        if (other == null) return; // <--- 這是防呆，加這行！ if (other == null) return; // <--- 這是防呆，加這行！

        Debug.Log($"[TreasureBox] 觸發物件: {other.gameObject.name}, Tag: {other.gameObject.tag}");

        if (other.CompareTag("testBox") || other.CompareTag("Player"))
        {
            exploded = true;
            Explode();
        }
    }

    private void Explode()
    {
        // Spawn BrokenBox (只是用來產生子物件，不真正留著)
        if (BrokenBoxPrefab.IsValid)
        {
            var brokenBox = Runner.Spawn(BrokenBoxPrefab, transform.position, transform.rotation);

            // 把子物件逐一 Spawn 成 NetworkObject
            foreach (Transform child in brokenBox.transform)
            {
                if (child.TryGetComponent<NetworkObject>(out var childNetObj))
                {
                    // 把碎片從 brokenBox 拿出來
                    child.SetParent(null);

                    // Spawn 成真正的 NetworkObject
                    Runner.Spawn(childNetObj, child.position, child.rotation);

                    // 加入清單
                    spawnedPieces.Add(childNetObj);

                    // 設定 Layer
                    childNetObj.gameObject.layer = LayerMask.NameToLayer("BrokenPiece");

                    // 加一點向上爆破力
                    if (childNetObj.TryGetComponent<Rigidbody>(out var rb))
                    {
                        rb.AddForce(Vector3.up * brokenUpwardForce, ForceMode.Impulse);
                    }
                }
                else
                {
                    Debug.LogWarning($"[TreasureBox] 子物件 {child.name} 沒有 NetworkObject，無法同步Spawn");
                }
            }

            // 最後把 BrokenBox本體 Despawn掉（因為已經沒用）
            Runner.Despawn(brokenBox);
        }

        // Spawn Special Item (Kreem)
        if (SpecialItemPrefab.IsValid)
        {
            var item = Runner.Spawn(SpecialItemPrefab, transform.position + new Vector3(0, 1f, 0), Quaternion.identity);
            if (item.TryGetComponent<Rigidbody>(out var itemRb))
            {
                Vector3 forceDirection = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                itemRb.AddForce(forceDirection.normalized * coinForce, ForceMode.Impulse);
            }
        }

        // 消除自己
        Runner.Despawn(Object);

        // 啟動碎片自動 Despawn
        StartCoroutine(DespawnSpawnedPieces());
    }

    private IEnumerator DespawnSpawnedPieces()
    {
        yield return new WaitForSeconds(brokenPieceDespawnTime);

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
