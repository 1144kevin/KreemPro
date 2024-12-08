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

    [Header("Bar Settings")]
    public BarSection barSection;  // This should be the BarSection for THIS player

    void Start()
    {
        rb = GetComponent<Rigidbody>();

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

        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log($"Hit by bullet on player: {gameObject.name}");
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