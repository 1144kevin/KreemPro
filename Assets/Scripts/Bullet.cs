using UnityEngine;
using TMPro;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]
    public GameObject bullet;
    public GameObject muzzleFlash;
    public Transform attackPoint;
    public float shootForce;
    public float bulletLifetime = 5f;
    public float bulletSpeed = 20f;
   
    [Header("Attack Settings")]
    public bool allowButtonHold;
    public float timeBetweenShooting;
    public int magazineSize;
    private int bulletsLeft;
    public float timeBetweenBulletIncrease = 1f;
    private bool shooting, readyToShoot, reloading;
    private bool allowInvoke = true; // bug fixing
    private float nextBulletIncreaseTime;

    [Header("UI Settings")]
    public AttackBar attackBar;

    private void Awake()
    {
        bulletsLeft = magazineSize;
        readyToShoot = true;
        nextBulletIncreaseTime = Time.time;
        attackBar = GetComponentInChildren<AttackBar>();
    }

    private void Update()
    {
        MyInput();
        CheckBulletIncrease();
    }

    private void MyInput()
    {
        // Check if allowed to hold down button and take corresponding input
        shooting = allowButtonHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

        // Shooting
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            Shoot();
        }
    }

    private void CheckBulletIncrease()
    {
        if (!shooting && !reloading && bulletsLeft < magazineSize && Time.time >= nextBulletIncreaseTime)
        {
            bulletsLeft = Mathf.Min(bulletsLeft + 1, magazineSize);     
            nextBulletIncreaseTime = Time.time + timeBetweenBulletIncrease;
            attackBar.UpdateAttackBar(bulletsLeft,magazineSize);
            
            
        }
    }

 void OnCollisionEnter(Collision collision)
{Debug.Log("Collision detected");
    if (collision.gameObject.CompareTag("Target"))
    {
        Debug.Log("hit");
        Destroy(gameObject); // Destroy the current bullet instance
    }
}   

    private void Shoot()
    {
        readyToShoot = false;

        // Instantiate bullet/projectile
        GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

        // Add force to the bullet
        Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
        bulletRb.AddForce(attackPoint.forward * bulletSpeed, ForceMode.Impulse);

        // Instantiate muzzle flash
        GameObject currentMuzzleFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

        // Destroy the bullet and muzzle flash after set times
        Destroy(currentBullet, bulletLifetime);
        Destroy(currentMuzzleFlash, 1f);

        bulletsLeft--;
        nextBulletIncreaseTime = Time.time + timeBetweenBulletIncrease;
        attackBar.UpdateAttackBar(bulletsLeft,magazineSize);
       

        if (allowInvoke)
        {
            Invoke("ResetShot", timeBetweenShooting);
            allowInvoke = false;
        }
    }

    private void ResetShot()
    {
        readyToShoot = true;
        allowInvoke = true;
    }
}
