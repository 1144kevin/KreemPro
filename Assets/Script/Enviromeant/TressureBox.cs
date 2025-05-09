using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class TreasureBox : NetworkBehaviour
{
    [Header("設定")]
    [SerializeField] private List<NetworkObject> brokenPiecePrefabs;
    [SerializeField] private NetworkPrefabRef specialItemPrefab;
    [SerializeField] private float respawnDelay = 5f;
    [SerializeField] private int spawnBatchSize = 10;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;
    [SerializeField] private ParticleSystem explosionEffect;

    [Networked]
    private bool IsVisible { get; set; }

    private bool exploded = false;
    private bool lastVisibleState = false;
    private float explosionForce = 10000f;

    private Vector3 boxPos;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            IsVisible = true;
            boxPos = transform.position;
        }

        lastVisibleState = IsVisible;
        SetTreasureBoxActive(IsVisible);

    }

    public override void Render()
    {
        // 手動比對可視狀態變化
        if (lastVisibleState != IsVisible)
        {
            lastVisibleState = IsVisible;
            SetTreasureBoxActive(IsVisible);
        }
    }
    public void TriggerExplosion(PlayerRef attacker)
    {
        if (!Object.HasStateAuthority || exploded) return;
        StartCoroutine(Explode(attacker));
    }
    private IEnumerator Explode(PlayerRef attacker)
    {
        exploded = true;
        IsVisible = false;


        // RpcPlayExplosionEffect();  // 所有人本地播放特效
        // 僅在攻擊者本地端播放音效
        RpcPlayExplosionSound();

        int count = 0;

        foreach (var prefab in brokenPiecePrefabs)
        {
            if (prefab == null) continue;

            Vector3 spawnOffset = new Vector3(
                Random.Range(-0.3f, 0.3f),
                Random.Range(0.05f, 0.15f),
                Random.Range(-0.3f, 0.3f)
            );

            Vector3 finalPosition = boxPos + spawnOffset;
            var piece = Runner.Spawn(prefab, finalPosition, Random.rotation);
            if (piece && piece.TryGetComponent<Rigidbody>(out var col))
            {
                Vector3 dir = Vector3.up + Random.insideUnitSphere;
                col.AddForce(dir.normalized * explosionForce, ForceMode.Impulse);
            }
            if (piece && piece.TryGetComponent<Collider>(out var coll))
            {
                coll.isTrigger = true;
            }

            count++;
            if (count % spawnBatchSize == 0)
            {
                yield return null;
            }
        }

        if (specialItemPrefab.IsValid)
        {
            var item = Runner.Spawn(specialItemPrefab, boxPos + Vector3.up, Quaternion.identity);
            if (item && item.TryGetComponent<Rigidbody>(out var rb))
            {
                Vector3 dir = Vector3.up + Random.insideUnitSphere;
                rb.AddForce(dir.normalized * explosionForce, ForceMode.Impulse);
            }
        }

        yield return new WaitForSeconds(respawnDelay);
        IsVisible = true;
        exploded = false;
    }

    private void SetTreasureBoxActive(bool active)
    {
        foreach (var r in GetComponentsInChildren<Renderer>(true))
            r.enabled = active;

        foreach (var c in GetComponentsInChildren<Collider>(true))
            c.enabled = active;
    }
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RpcPlayExplosionSound()
    {
        if (Runner.LocalPlayer == null) return; // 保險

        sceneAudioSetter?.PlayBoxExplodeSound();
    }
    // [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    // private void RpcPlayExplosionEffect()
    // {
    //     explosionEffect?.Play();
    // }

}