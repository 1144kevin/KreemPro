using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Fusion;
using UnityEngine;
using UnityEngine.Serialization;

public class AttackHandler : NetworkBehaviour
{
    [Header("攻擊特效群組")]
    [Tooltip("動畫 1 播的特效 (1,2)")]
    [SerializeField] private ParticleSystem[] effectsForAnim1;
    [Tooltip("動畫 2 播的特效 (3,4)")]
    [SerializeField] private ParticleSystem[] effectsForAnim2;
    [SerializeField] private Transform CharacterTrans;
    [SerializeField] private LayerMask HitLayer = ~0;
    [SerializeField] private HitOptions HitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
    [SerializeField] private int damage = 10;
    [SerializeField] private int attackDistance = 20;
    [SerializeField] private float attackDelay = 0f; // 攻擊延遲時間
    [SerializeField] private ObjectSpawner objectSpawner;
    // 將攻擊動畫類型定義成 enum，更直覺：
    public enum AttackAnimType { Anim1 = 0, Anim2 = 1 }

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
        objectSpawner?.SpawnSphere();
        StartCoroutine(DespawnAfterDelay(3f));
        // 延遲傷害判定
        // PerformAttack(origin, direction);
        Invoke(nameof(PerformAttack), attackDelay);
    }

    public void PlayEffect_Anim1()
    {
        if (!Object.HasStateAuthority) return;
        Rpc_PlayAttackEffect((int)AttackAnimType.Anim1);
    }

    public void PlayEffect_Anim2()
    {
        if (!Object.HasStateAuthority) return;
        Rpc_PlayAttackEffect((int)AttackAnimType.Anim2);
    }

    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_PlayAttackEffect(int animType)
    {
        var group = animType == (int)AttackAnimType.Anim1
                    ? effectsForAnim1
                    : effectsForAnim2;
        foreach (var ps in group)
        {
            ps?.Stop();
            ps?.Play();
        }
    }

    public void Attack()
    {
        if (HasInputAuthority)
        {
            Rpc_RequestAttack();
        }
    }

    private void PerformAttack(Vector3 origin, Vector3 direction)
    {
        Vector3 rayOrigin = origin + Vector3.up * 100f; // 角色的位置 + 一點高度
        //Debug.DrawRay(rayOrigin, CharacterTrans.forward * attackDistance, Color.red, 1f);
        if (Runner.LagCompensation.Raycast(
            rayOrigin,
            CharacterTrans.forward,
            attackDistance,
            Object.InputAuthority,
            out var hit,
            HitLayer,
            HitOptions))
        {
            Debug.Log(hit.GameObject.name);
            if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
            {
                hitPlayer.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("hit fail");
        }

        // 生成子彈
        StartCoroutine(spawnBullet(origin, direction));
        // var bullet = objectSpawner.SpawnShot(shotOrigin, Quaternion.LookRotation(direction));
        // if (bullet != null)
        // {
        //     bullet.Fire(direction);
        // }

        //StartCoroutine(DespawnAfterDelay(3f));
    }

    private IEnumerator spawnBullet(Vector3 origin, Vector3 direction)
    {
        string playerName = gameObject.name;
        Debug.Log($"Spawning bullet for player: {playerName}");

        if (playerName == "Robot")
        {
            Vector3 shotOrigin = origin + Vector3.up * 150f;
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            float offsetDistance = 70f;
            // 再打右邊子彈
            Vector3 rightShotOrigin = shotOrigin + right * offsetDistance;
            var rightBullet = objectSpawner.SpawnShot(rightShotOrigin, Quaternion.LookRotation(direction));
            if (rightBullet != null)
            {
                rightBullet.Fire(direction);
            }

            // 等0.6秒
            yield return new WaitForSeconds(0.6f);
            
            // 先打左邊子彈
            Vector3 leftShotOrigin = shotOrigin - right * offsetDistance;
            var leftBullet = objectSpawner.SpawnShot(leftShotOrigin, Quaternion.LookRotation(direction));
            if (leftBullet != null)
            {
                leftBullet.Fire(direction);
            }

        }
        else if (playerName == "Mushroom")
        {
            Vector3 shotOrigin = origin + Vector3.up * 100f;
            var bullet = objectSpawner.SpawnShot(shotOrigin, Quaternion.LookRotation(direction));
            if (bullet != null)
            {
                bullet.Fire(direction);
            }
        }
    }


    private IEnumerator DespawnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);  // 等待指定秒數
           if (objectSpawner != null)
        {
            objectSpawner.DespawnAll();  // 3 秒後執行
        }
    }


}
