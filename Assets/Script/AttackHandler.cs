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
    [SerializeField] private LayerMask HitLayer = ~0;
    [SerializeField] private HitOptions HitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
    [SerializeField] private int damage = 10;
    [SerializeField] private int attackDistance = 20;

    [SerializeField] private ObjectSpawner objectSpawner;


    private void Awake()
    {
        if (objectSpawner == null)
            objectSpawner = GetComponent<ObjectSpawner>();

        if (CharacterTrans == null)
            CharacterTrans = transform;

    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestAttack()
    {
        PerformAttack(); // Host 幫忙做 Raycast 與傷害處理
    }

    public void Attack()
    {
        if (HasInputAuthority)
        {
            Rpc_RequestAttack(); // 傳給 Host 處理
        }
    }

    private void PerformAttack()
    {
        // if (Runner.LagCompensation == null)
        // {
        //     Debug.LogError("LagCompensation is null! This should only run on Host.");
        // }
        //Debug.Log($"[Runner Check] Runner: {Runner}, LagCompensation: {Runner.LagCompensation}, Runner.Mode: {Runner?.Mode}");
        objectSpawner.SpawnSphere();
        StartCoroutine(DespawnAfterDelay(3f));

        Vector3 rayOrigin = CharacterTrans.position + Vector3.up * 100f;
        Debug.DrawRay(rayOrigin, CharacterTrans.forward * attackDistance, Color.red, 1f);

        if (Runner.LagCompensation.Raycast(
            rayOrigin,
            CharacterTrans.forward,
            attackDistance,
            Object.InputAuthority,
            out var hit,
            HitLayer,
            HitOptions))
        {
            Debug.Log("hit");
            if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
            {
                Debug.Log(hitPlayer.gameObject.name);
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
    }


}
