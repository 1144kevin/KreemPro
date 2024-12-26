// RobotBullet.cs
using UnityEngine;

public class RobotBullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 2f;
    public float attackLost = 20f;
    private Vector3 moveDirection;
    private Attack playerAttack;

    private void OnEnable()
{
    playerAttack = null;  // Reset reference when bullet is enabled
}

public void Initialize(Vector3 targetPosition, Attack ownerAttack)
{
    if (ownerAttack != null && ownerAttack.CanFire())
    {
        playerAttack = ownerAttack;
        moveDirection = (targetPosition - transform.position).normalized;
        Destroy(gameObject, lifeTime);
        playerAttack.AttackLost(attackLost);
    }
    else
    {
        Destroy(gameObject);
    }
}

    void Update()
    {   
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet") { }
        Destroy(gameObject);
    }
}