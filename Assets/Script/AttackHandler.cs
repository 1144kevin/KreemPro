using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackHandler : NetworkBehaviour
{
    [SerializeField] private Transform CharacterTrans;
    [SerializeField] private LayerMask HitLayer;
    [SerializeField] private HitOptions HitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
    [SerializeField] private int damage = 10;

    [SerializeField] private ObjectSpawner objectSpawner;
    private void Awake()
    {

    }
    public void Attack()
    {
        //Debug.Log("shot1");
        objectSpawner.SpawnSphere();

        StartCoroutine(DespawnAfterDelay(3f));
        // if (Runner.LagCompensation.Raycast(
        //     CharacterTrans.position, CharacterTrans.forward,
        //     Mathf.Infinity,
        //     Object.InputAuthority,
        //     out LagCompensatedHit hit,
        //     HitLayer,
        //     HitOptions))
        // {
        //     if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
        //     {
        //         Debug.Log(hitPlayer.gameObject.name);
        //         hitPlayer.TakeDamage(damage);
        //     }
        // }
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestAttack([RpcTarget] PlayerRef requestingPlayer)
    {
        PerformAttack(requestingPlayer);
    }
    private void PerformAttack(PlayerRef shooter)
{
    if (Runner.LagCompensation == null)
    {
        Debug.LogError("LagCompensation is null! This should only run on Host.");
        return;
    }

    Vector3 rayOrigin = CharacterTrans.position + Vector3.up * 1.0f;
    Debug.DrawRay(rayOrigin, CharacterTrans.forward * attackDistance, Color.red, 1f);

    if (Runner.LagCompensation.Raycast(
        rayOrigin,
        CharacterTrans.forward,
        attackDistance,
        shooter,
        out var hit,
        HitLayer,
        HitOptions))
    {
        Debug.Log("hit: " + hit.GameObject.name);
        if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
        {
            hitPlayer.TakeDamage(damage);
        }
    }
    else
    {
        Debug.Log("hit fail");
    }
}

    private IEnumerator DespawnAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);  // 等待指定秒數
        objectSpawner.DespawnAll();  // 3 秒後執行
        //Debug.Log("despawn");
    }


}
