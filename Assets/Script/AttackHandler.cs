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
    [SerializeField] private Vector3 attackRange = new Vector3(200f, 200f, 400f);
    //private Vector3 boxHalfExtents = new Vector3(200f, 400f, 400f);//數字再調

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
        if (HasInputAuthority)
        {
            // 本地自己播攻擊音效
            if (sceneAudioSetter != null)
            {
                var clip = sceneAudioSetter.GetAttackSFXByCharacterIndex(characterSoundIndex);
                if (clip != null)
                {
                    AudioManager.Instance.PlaySFX(clip);
                }
            }
        }

        if (!Object.HasStateAuthority) return;

        Rpc_PlayAttackEffect((int)AttackAnimType.Anim1);
    }


    public void PlayEffect_Anim2()
    {
        if (HasInputAuthority)
        {
            if (sceneAudioSetter != null)
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
            ps?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            ps?.Play();
        }
    }

    private void PerformAttack(Vector3 rayorigin, Vector3 direction)
    {
        Quaternion attackQuaternion = Quaternion.LookRotation(CharacterTrans.forward);
        string playerName = gameObject.name;
        if (playerName == "Robot" || playerName == "Mushroom")
        {
            //Debug.DrawRay(rayorigin, CharacterTrans.forward * attackDistance, Color.red, 1f);

            if (Runner.LagCompensation.Raycast(
                rayorigin,
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
                    if (Object.InputAuthority != hitPlayer.Object.InputAuthority)
                    {
                        hitPlayer.TakeDamage(damage);
                    }
                }
            }
        }
        else
        {
            rayorigin=rayorigin+CharacterTrans.forward*200f;
            //Debug.DrawRay(rayorigin, CharacterTrans.forward * attackRange.z, Color.red, 1f);
            
            List<LagCompensatedHit> hits = new List<LagCompensatedHit>();
            if (Runner.LagCompensation.OverlapBox(
                rayorigin,
                attackRange,
                attackQuaternion,
                Object.InputAuthority,
                hits,
                HitLayer,
                HitOptions) > 0)
            {
                foreach (var hit in hits)
                {
                    Debug.Log(hit.GameObject.name);

                    if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
                    {
                        if (Object.InputAuthority != hitPlayer.Object.InputAuthority)
                        {
                            hitPlayer.TakeDamage(damage);
                        }
                    }
                }
                
            };
        }

    }
    // private void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     Vector3 forward = CharacterTrans.forward;
    //     Vector3 center = CharacterTrans.position+CharacterTrans.forward*200f+Vector3.up * 150f;
    //     Quaternion rotation = Quaternion.LookRotation(forward);
    //     Vector3 size = new Vector3(200f, 200f, 400f);

    //     Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
    //     Gizmos.DrawWireCube(Vector3.zero, size);
    // }


    private IEnumerator spawnBullet()
    {
        yield return new WaitForSeconds(attackDelay);

        string playerName = gameObject.name;
        Vector3 direction = CharacterTrans.forward;

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
            Vector3 shotOrigin = CharacterTrans.position + Vector3.up * 100f;
            var bullet = objectSpawner.SpawnShot(shotOrigin, Quaternion.LookRotation(direction));
            if (bullet != null)
            {
                bullet.Fire(direction);
                PerformAttack(shotOrigin, direction);
            }

            //DespawnAfterDelay(5f);
        }
        else
        {
            Vector3 shotOrigin = CharacterTrans.position+Vector3.up * 100f;
            PerformAttack(shotOrigin, direction);
        }


    }
public Transform GetCharacterTrans()
{
    return CharacterTrans;
}

}
