// AttackEffect.cs
using UnityEngine;
using System.Collections;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] private ParticleSystem attackEffect;
    [SerializeField] private ParticleSystem attackEffect2;
    [SerializeField] private ParticleSystem runAttackEffect;
    [SerializeField] private ParticleSystem runAttackEffect2;
    [SerializeField] private Transform leftShootPoint;
    [SerializeField] private Transform rightShootPoint;
    [SerializeField] private Transform leftRunShootPoint;
    [SerializeField] private Transform rightRunShootPoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform target;
    public float bulletSpawnDelay = 1.5f;
    private Attack attackBar;
    public float attackLostLp = 10f;

    void Start()
{
    attackBar = transform.GetComponent<Attack>();
    if (attackBar == null)
    {
        attackBar = transform.GetComponentInParent<Attack>();
    }
}

    public void PlayAttackEffect()
{
    if (attackEffect != null && !attackEffect.isPlaying && attackBar.CanFire())
    {
        attackEffect.Play();
        
        // Check player tag and apply attack loss
        if (gameObject.CompareTag("Eagle") || gameObject.CompareTag("Leopard"))
        {
            attackBar.AttackLost(attackLostLp);
        }
        
        StartCoroutine(ShootBulletWithDelay(rightShootPoint));
    }
}

    public void PlayAttackEffect2()
    {
        if (attackEffect2 != null && !attackEffect2.isPlaying && attackBar.CanFire())
        {
            attackEffect2.Play();
            ShootBullet(leftShootPoint);
        }
    }

    public void PlayRunAttackEffect()
    {
        if (runAttackEffect != null && !runAttackEffect.isPlaying && attackBar.CanFire())
        {
            runAttackEffect.Play();
            ShootBullet(rightRunShootPoint);
        }
    }

    public void PlayRunAttackEffect2()
    {
        if (runAttackEffect2 != null && !runAttackEffect2.isPlaying && attackBar.CanFire())
        {
            runAttackEffect2.Play();
            ShootBullet(leftRunShootPoint);
        }
    }

    public void StopAttackEffect()
    {
        if (attackEffect != null && attackEffect.isPlaying)
        {
            Debug.Log("Stop");
            attackEffect.Stop();
        }
    }

    public void StopAttackEffect2()
    {
        if (attackEffect2 != null && attackEffect2.isPlaying)
        {
            Debug.Log("Stop");
            attackEffect2.Stop();
        }
    }

    public void StopRunAttackEffect()
    {
        if (runAttackEffect != null && !runAttackEffect.isPlaying)
        {
            Debug.Log("Stop");
            runAttackEffect.Stop();
        }
    }

    public void StopRunAttackEffect2()
    {
        if (runAttackEffect2 != null && !runAttackEffect2.isPlaying)
        {
            Debug.Log("Stop");
            runAttackEffect2.Stop();
        }
    }

    private IEnumerator ShootBulletWithDelay(Transform shootPoint)
    {
        yield return new WaitForSeconds(bulletSpawnDelay);
        if (attackBar.CanFire())
        {
            ShootBullet(shootPoint);
        }
    }

    private void ShootBullet(Transform shootPoint)
    {
        if (bulletPrefab == null || shootPoint == null)
        {
            return;
        }

        GameObject bullet = Instantiate(bulletPrefab, shootPoint.position, Quaternion.identity);
        if (target != null)
        {
            bullet.GetComponent<RobotBullet>().Initialize(target.position, attackBar);
        }
        else
        {
            Debug.LogWarning("No target assigned for the bullet!");
        }
    }
}