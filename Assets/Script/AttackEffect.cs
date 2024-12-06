using UnityEngine;
using System.Collections;

public class AttackEffect : MonoBehaviour
{
    public ParticleSystem attackEffect;
    public ParticleSystem attackEffect2;
    public ParticleSystem runAttackEffect;
    public ParticleSystem runAttackEffect2;
    public Transform leftShootPoint;
    public Transform rightShootPoint;
    public Transform leftRunShootPoint;
    public Transform rightRunShootPoint;
    public GameObject bulletPrefab;
    public Transform target; // 子彈射擊目標
    public float bulletSpawnDelay = 1.5f; // 子彈生成延遲時間

    // 發射右手攻擊
    public void PlayAttackEffect()
    {
        if (attackEffect != null && !attackEffect.isPlaying)
        {
            attackEffect.Play();
            StartCoroutine(ShootBulletWithDelay(rightShootPoint));
        }
    }

    // 發射左手攻擊
    public void PlayAttackEffect2()
    {
        if (attackEffect2 != null && !attackEffect2.isPlaying)
        {
            attackEffect2.Play();
            ShootBullet(leftShootPoint);
        }
    }

    // 發射右手奔跑攻擊
    public void PlayRunAttackEffect()
    {
        if (runAttackEffect != null && !runAttackEffect.isPlaying)
        {
            runAttackEffect.Play();
            ShootBullet(rightRunShootPoint);
        }
    }

    // 發射左手奔跑攻擊
    public void PlayRunAttackEffect2()
    {
        if (runAttackEffect2 != null && !runAttackEffect2.isPlaying)
        {
            runAttackEffect2.Play();
            ShootBullet(leftRunShootPoint);
        }
    }

    // 可選：添加一個停止特效的方法，如果需要在其他地方手動停止
    public void StopAttackEffect()
    {
        if (attackEffect != null && attackEffect.isPlaying)
        {
            Debug.Log("Stop");
            attackEffect.Stop();  // 停止播放
        }
    }

    public void StopAttackEffect2()
    {
        if (attackEffect2 != null && attackEffect2.isPlaying)
        {
            Debug.Log("Stop");
            attackEffect2.Stop();  // 停止播放
        }
    }
    public void StopRunAttackEffect()
    {
        if (runAttackEffect != null && !runAttackEffect.isPlaying)
        {
            Debug.Log("Stop");
            runAttackEffect.Stop();  // 觸發播放
        }
    }
    public void StopRunAttackEffect2()
    {
        if (runAttackEffect2 != null && !runAttackEffect2.isPlaying)
        {
            Debug.Log("Stop");
            runAttackEffect2.Stop();  // 觸發播放
        }
    }
    private IEnumerator ShootBulletWithDelay(Transform shootPoint)
    {
        yield return new WaitForSeconds(bulletSpawnDelay); // 等待指定的延遲時間
        ShootBullet(shootPoint);
    }
    private void ShootBullet(Transform shootPoint)
    {
        // 生成子彈
        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);

        // 初始化子彈的目標方向
        if (target != null)
        {
            bullet.GetComponent<RobotBullet>().Initialize(target.position);
        }
        else
        {
            Debug.LogWarning("No target assigned for the bullet!");
        }
    }
}
