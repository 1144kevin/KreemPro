using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class AttackHandler : NetworkBehaviour
{
    [SerializeField] private Transform CharacterTrans;
    [SerializeField] private LayerMask HitLayer;
    [SerializeField] private HitOptions HitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
    [SerializeField] private int damage = 10;
    private void Awake()
    {
        if (CharacterTrans == null)
        {
            Debug.LogError("CharacterTrans is null in Awake!");
        }
    }
    public void Attack()
    {
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
        //         // Debug.Log(hitPlayer.gameObject.name);
        //         hitPlayer.TakeDamage(damage);
        //         // if (Input.GetKey(KeyCode.Space))
        //         // {
        //         //     hitPlayer.TakeDamage(damage);
        //         // }

        //     }
        // }
    }
}
