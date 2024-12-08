using UnityEngine;

public class RobotBullet : MonoBehaviour
{
    public float speed = 40f;
    public float lifeTime = 2f;
    public float damage = 20f;
    public float attackCost = 50f;  // How much attack power each bullet consumes
    private Vector3 moveDirection;
    private BarSection robotBarSection;  // Reference to the robot's BarSection

    public void Initialize(Vector3 targetPosition)
    {
        // Try to find the BarSection from the scene
        // First, try to find it on the object that created the bullet
        GameObject robot = GameObject.FindWithTag("Robot"); // Make sure your robot has the "Robot" tag
        if (robot != null)
        {
            robotBarSection = robot.GetComponent<BarSection>();
            if (robotBarSection == null)
            {
                robotBarSection = robot.GetComponentInChildren<BarSection>();
            }
            
            if (robotBarSection != null)
            {
                Debug.Log($"Current Attack Power: {robotBarSection.GetCurrentAttack()}");
                if (robotBarSection.GetCurrentAttack() >= attackCost)
                {
                    moveDirection = (targetPosition - transform.position).normalized;
                    robotBarSection.ModifyAttack(-attackCost);  // Reduce attack power
                    Debug.Log($"Attack power reduced by {attackCost}. New attack power: {robotBarSection.GetCurrentAttack()}");
                    Destroy(gameObject, lifeTime);
                }
                else
                {
                    Debug.Log("Not enough attack power to shoot!");
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                Debug.LogWarning("BarSection not found on Robot!");
            }
        }
        else
        {
            Debug.LogWarning("Robot not found in scene!");
            // Fallback behavior
            moveDirection = (targetPosition - transform.position).normalized;
            Destroy(gameObject, lifeTime);
        }
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHitted = collision.gameObject.GetComponent<PlayerHitted>();
            if (playerHitted != null)
            {
                Debug.Log($"Bullet hit player with damage: {damage}");
            }
        }
        
        Destroy(gameObject);
    }
}