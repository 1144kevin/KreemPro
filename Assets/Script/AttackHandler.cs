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
    private IEnumerator DespawnAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);  // 等待指定秒數
        objectSpawner.DespawnAll();  // 3 秒後執行
        //Debug.Log("despawn");
    }


}
