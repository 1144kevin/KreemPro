using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class PlayerController : MonoBehaviour
{
    public float speed = 5.0f;
    private float originalSpeed; // 保存初始速度
    public float rotationSpeed = 720.0f;
    public float detectionRadius = 45.0f; // 偵測範圍的半徑

    public float customGravity = 30.0f;

    private Rigidbody rb;
    private Animator animator;
    private Transform target; // 當前偵測的目標物體

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        originalSpeed = speed;  // 初始化 originalSpeed

        // 添加一個 SphereCollider 作為偵測範圍的觸發區域
        SphereCollider detectionCollider = gameObject.AddComponent<SphereCollider>();
        detectionCollider.isTrigger = true;
        detectionCollider.radius = detectionRadius;
    }

    void Update()
    {
        // // 獲取當前動畫狀態
        // AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 為 base layer
        // bool isInAttackAnimation = stateInfo.IsName("Attack") || stateInfo.IsName("Attack0"); // "Attack" 為動畫狀態名稱，需與 Animator 中一致
        bool Moving = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.DownArrow) ||
                   Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow);
        // 自定義重力
        rb.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);
        // 獲取輸入
        float moveX = 0; // 左右移動 (A/D 或 左右方向鍵)
        float moveZ = 0;   // 前後移動 (W/S 或 上下方向鍵)

        // 移動方向
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            moveX = -1;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            moveX = 1;
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            moveZ = 1;
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            moveZ = -1;
        }

        Vector3 move = new Vector3(-moveX, 0, -moveZ).normalized;

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
        else if (target != null && Input.GetKey(KeyCode.K))
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

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (Moving)
            {
                // 如果正在移動且按下攻擊鍵，觸發跑攻擊動畫
                animator.SetTrigger("isRunAttack");
            }
            else
            {
                // 否則觸發普通攻擊動畫
                animator.SetTrigger("isAttack");
            }
        }
        else
        {
            animator.SetTrigger("unAttack");
            animator.SetTrigger("unRunAttack");
        }
    }

    // 偵測目標物體進入範圍
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target")) // 假設目標物體的 Tag 是 "Target"
        {
            target = other.transform; // 設定目標為觸發的物體
        }
    }

    // 偵測目標物體離開範圍
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Target") && other.transform == target)
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
