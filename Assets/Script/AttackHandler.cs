using System;
using System.Collections;
using System.Collections.Generic;
//using System.Diagnostics;
using Fusion;
using Unity.VisualScripting;
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
    [SerializeField] private SceneAudioSetter sceneAudioSetter;
    [SerializeField] private int characterSoundIndex = 0; // 攻擊音效用的角色 ID

    // 將攻擊動畫類型定義成 enum，更直覺：
    public enum AttackAnimType { Anim1 = 0, Anim2 = 1 }

    private void Awake()
    {
        if (objectSpawner == null)
            objectSpawner = GetComponent<ObjectSpawner>();

        if (CharacterTrans == null)
            CharacterTrans = transform;

    }

    public void Attack()
    {
        if (HasInputAuthority)
        {
            Rpc_RequestAttack();
        }
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void Rpc_RequestAttack()
    {
        StartCoroutine(spawnBullet());
    }

    public void PlayEffect_Anim1()
    {
        if (!Object.HasStateAuthority) return;
        Rpc_PlayAttackEffect((int)AttackAnimType.Anim1);
        if (sceneAudioSetter != null) //攻擊音效
        {
            var clip = sceneAudioSetter.GetAttackSFXByCharacterIndex(characterSoundIndex);
            if (clip != null)
            {
                AudioManager.Instance.PlaySFX(clip);
            }
        }
    }

    public void PlayEffect_Anim2()
    {
        if (!Object.HasStateAuthority) return;

        Rpc_PlayAttackEffect((int)AttackAnimType.Anim2);

        if (sceneAudioSetter != null) //攻擊音效
        {
            if (characterSoundIndex == 0)
            {
                sceneAudioSetter?.PlayOneShotSound();
            }
            else
            {
                var clip = sceneAudioSetter.GetAttackSFXByCharacterIndex(characterSoundIndex);
                if (clip != null)
                {
                    AudioManager.Instance.PlaySFX(clip);
                }
            }
        }
    }


    [Rpc(sources: RpcSources.StateAuthority, targets: RpcTargets.All)]
    private void Rpc_PlayAttackEffect(int animType)
    {
        var group = animType == (int)AttackAnimType.Anim1
                    ? effectsForAnim1
                    : effectsForAnim2;
        foreach (var ps in group)
        {
            ps?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps?.Play();
        }
    }

    private void PerformAttack(Vector3 origin, Vector3 direction)
    {
        Vector3 rayOrigin = origin; 
        Debug.DrawRay(rayOrigin, CharacterTrans.forward * attackDistance, Color.red, 1f);

        if (Runner.LagCompensation.Raycast(
            rayOrigin,
            direction,
            attackDistance,
            Object.InputAuthority,
            out var hit,
            HitLayer,
            HitOptions))
        {
            Debug.Log(hit.GameObject.name);

            if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
            {
                if (hitPlayer.gameObject.name != gameObject.name && Object.InputAuthority != hitPlayer.Object.InputAuthority)
                {
                    hitPlayer.TakeDamage(damage);
                }
            }
        }

    }

    private IEnumerator spawnBullet()
    {
        yield return new WaitForSeconds(attackDelay);

        string playerName = gameObject.name;
        Vector3 direction=CharacterTrans.forward;

        if (playerName == "Robot")
        {
            Vector3 shotOrigin = CharacterTrans.position + Vector3.up * 150f + direction * 150f;
            Vector3 right = Vector3.Cross(Vector3.up, direction).normalized;
            float offsetDistance = 70f;
            // 再打右邊子彈
            Vector3 rightShotOrigin = shotOrigin + right * offsetDistance;
            var rightBullet = objectSpawner.SpawnShot(rightShotOrigin, Quaternion.LookRotation(direction));
            if (rightBullet != null)
            {
                rightBullet.Fire(direction);
                PerformAttack(rightShotOrigin, direction);
            }

            yield return new WaitForSeconds(0.2f);

            // 先打左邊子彈
            Vector3 leftShotOrigin = shotOrigin - right * offsetDistance;
            var leftBullet = objectSpawner.SpawnShot(leftShotOrigin, Quaternion.LookRotation(direction));
            if (leftBullet != null)
            {
                leftBullet.Fire(direction);
                PerformAttack(leftShotOrigin, direction);
            }

            

        }
        else if (playerName == "Mushroom")
        {
            Vector3 shotOrigin =  CharacterTrans.position + Vector3.up * 100f;
            var bullet = objectSpawner.SpawnShot(shotOrigin, Quaternion.LookRotation(direction));
            if (bullet != null)
            {
                bullet.Fire(direction);
                PerformAttack(shotOrigin, direction);
            }

            //DespawnAfterDelay(5f);
        }
        else{
            Vector3 shotOrigin =  CharacterTrans.position  + Vector3.up * 100f;
            PerformAttack(shotOrigin, direction);
        }


    }
}
