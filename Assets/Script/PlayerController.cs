using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    public float rotationSpeed = 720.0f;
    public float detectionRadius = 45.0f; // 偵測範圍的半徑

    public float customGravity = 30.0f;

    private Rigidbody rb;
    private Animator animator;
    private Transform target; // 當前偵測的目標物體
    private Vector2 moveInput;
    private bool attackInput;
    private bool isAttacking = false; // 防止動畫重複觸發
    PlayerActions controls;

    void Awake()
    {
        // 初始化 Input Actions
        controls = new PlayerActions();

        // 訂閱移動輸入事件
        controls.PlayerControls.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.PlayerControls.Movement.canceled += ctx => moveInput = Vector2.zero;

        // 訂閱攻擊輸入事件
        controls.PlayerControls.Attack.started += ctx => attackInput = true;
        controls.PlayerControls.Attack.canceled += ctx => attackInput = false;

    }
    void OnEnable()
    {
        controls.PlayerControls.Enable();
    }

    void OnDisable()
    {
        controls.PlayerControls.Disable();
    }
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // 添加一個 SphereCollider 作為偵測範圍的觸發區域
        SphereCollider detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    void Update()
    {
        // 自定義重力
        rb.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);
        Vector3 move = new Vector3(-moveInput.x, 0, -moveInput.y).normalized;

        bool isMoving = move.magnitude > 0.1f;

        if (isMoving)
        {
            // 使用玩家的輸入方向進行旋轉
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            // 設定角色的前進方向，保留 Y 軸速度
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
            // 停止水平移動，但保留 Y 軸速度
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
}

