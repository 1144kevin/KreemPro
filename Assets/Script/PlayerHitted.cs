using UnityEngine;

public class PlayerHitted : MonoBehaviour
{
    public float pushForce = 10f;
    public float pushDuration = 2f;
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    public ParticleSystem HittedEffect;

    public int damageAmount = 20; // 每次被擊中扣的血量
    private Health playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<Health>();

        // 停止特效，確保它在開始時不運行
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
        
        if (barSection == null)
        {
            Debug.LogError($"BarSection not assigned to PlayerHitted on GameObject: {gameObject.name}");
            // Try to find BarSection on this object or its children
            barSection = GetComponentInChildren<BarSection>();
            if (barSection == null)
            {
                Debug.LogError("Could not find BarSection component automatically!");
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"Collision detected with: {collision.gameObject.name} (Tag: {collision.gameObject.tag})");

        if (isPushed)
        {
            if (collision.gameObject.tag != "Bullet")
            {
                Vector3 collisionNormal = collision.contacts[0].normal;
                pushDirection = Vector3.Reflect(pushDirection, collisionNormal);

                float remainingPushTime = pushEndTime - Time.time;
                pushEndTime = Time.time + remainingPushTime;
            }
        }

        // 檢查碰撞對象是否是玩家的手或其他需要觸發的對象
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("hit");
            // 如果存在 Health 組件，扣血
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            // 計算碰撞方向（從碰撞點推開圓柱體）
            pushDirection = transform.position - collision.transform.position;
            pushDirection = pushDirection.normalized;

            isPushed = true;
            if (HittedEffect != null)
            {
                HittedEffect.Play();
            }
            pushEndTime = Time.time + pushDuration;

            if (barSection != null)
            {
                float previousHealth = barSection.GetCurrentHealth();
                barSection.TakeDamage(30);
                float newHealth = barSection.GetCurrentHealth();
                Debug.Log($"Player {gameObject.name} - Health changed from {previousHealth} to {newHealth}");
            }
            else
            {
                Debug.LogError($"BarSection is null on player: {gameObject.name}");
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Trigger detected with: {other.gameObject.name} (Tag: {other.gameObject.tag})");
        
        if (other.gameObject.CompareTag("MineEffect"))
        {
            Debug.Log($"Mine hit on player: {gameObject.name}");
            if (barSection != null)
            {
                float previousHealth = barSection.GetCurrentHealth();
                barSection.TakeDamage(30);
                float newHealth = barSection.GetCurrentHealth();
                Debug.Log($"Player {gameObject.name} - Health changed from {previousHealth} to {newHealth}");
            }
            else
            {
                Debug.LogError($"BarSection is null on player: {gameObject.name}");
            }
        }
    }

    void Update()
    {
        if (isPushed && Time.time <= pushEndTime)
        {
            pushDirection.y = 0;
            Vector3 newPosition = rb.position + pushDirection * pushForce * Time.deltaTime;
            newPosition.y = rb.position.y;

            rb.MovePosition(newPosition);
        }
        else if (isPushed && Time.time > pushEndTime)
        {
            StopPush();
        }
    }

    private void StopPush()
    {
        isPushed = false;
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
    }
}