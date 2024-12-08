using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private float originalSpeed; // 保存初始速度
    public float rotationSpeed = 720.0f;
    public float detectionRadius = 45.0f;
    public float customGravity = 30.0f;

    private Rigidbody rb;
    private Animator animator;
    private Transform target;
    private Vector2 moveInput = Vector2.zero;
    private bool attackInput = false;
    private bool isAttacking = false;
    [SerializeField] private int playerIndex = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        originalSpeed = speed;  // 初始化 originalSpeed

        SphereCollider detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }
    public int GetPlayerIndex()
    {
        return playerIndex;
    }

    private void FixedUpdate()
    {
        // Apply gravity
        rb.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);

        // Handle movement
        Vector3 move = new Vector3(-moveInput.x, 0, -moveInput.y).normalized;
        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.velocity = new Vector3(transform.forward.x * speed, rb.velocity.y, transform.forward.z * speed);
            animator.SetBool("isMoving", true);
        }
        else if (target != null && attackInput)
        {
            // 當攻擊鍵被按下且有偵測目標時，自動面向目標方向
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            directionToTarget.y = 0; // 忽略Y軸，僅水平旋轉
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 1500 * Time.deltaTime);

            rb.velocity = new Vector3(0, rb.velocity.y, 0); // 停止水平移動，但保留 Y 軸速度
            animator.SetBool("isMoving", false);
        }
        else
        {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
            animator.SetBool("isMoving", false);
        }
        if (attackInput && !isAttacking) // 確保攻擊動畫只觸發一次
        {
            isAttacking = true;

            if (isMoving)
            {
                animator.SetTrigger("isRunAttack");
            }
            else
            {
                animator.SetTrigger("isAttack");
            }
        }
        else if (!attackInput) // 當按鍵釋放時重置狀態
        {
            isAttacking = false;
            animator.SetTrigger("unAttack");
            animator.SetTrigger("unRunAttack");
        }
    }
    // 偵測目標物體進入範圍


    public void SetInputVector(Vector2 input)
    {
        moveInput = input;
    }

    public void SetAttackInput(bool input)
    {
        attackInput = input;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // 假設目標物體的 Tag 是 "Target"
        {
            target = other.transform; // 設定目標為觸發的物體
        }
    }

    // 偵測目標物體離開範圍
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && other.transform == target)
        {
            target = null; // 當目標離開範圍時，將目標設為 null
        }
    }
    public void IncreaseSpeed(float amount)
    {
        speed += amount;
        Debug.Log($"玩家速度增加，當前速度為：{speed}");

         StartCoroutine(RestoreSpeedAfterDelay(10f));
    }

     private IEnumerator RestoreSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        speed = originalSpeed;
        Debug.Log($"玩家速度還原，當前速度為：{speed}");

    }
}