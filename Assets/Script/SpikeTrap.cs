using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10; // 将damage改为int并赋值默认值
    public List<PlayerController> ListCharacters = new List<PlayerController>();

    [SerializeField] private Animator animator;
    [SerializeField] private float pushForce = 5f; // 玩家被推的力量

    public float pushDuration = 2f; // 持續時間2秒
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    private Coroutine currentCoroutine;
    private void Start()
    {
        isPushed = false;
    }
    private void OnTriggerEnter(Collider other)
{
    PlayerController control = other.GetComponent<PlayerController>();

    if (!isPushed)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // 停止之前的協程
        }

        if (control != null && control.CompareTag("Player"))
        {
            isPushed = true;
            animator.SetBool("playerOn", true); // 设置动画状态为 true
            currentCoroutine = StartCoroutine(TriggerTrap(other));
        }
    }
}
    
    private IEnumerator TriggerTrap(Collider other)
    {
        
        yield return new WaitForSeconds(1.0f);
        if (other.CompareTag("Player"))
        {
            Health playerHealth = other.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }

            // 給玩家一個推力
            Rigidbody playerRigidbody = other.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {

                // 計算推力方向（使玩家遠離陷阱中心）
                Vector3 pushDirection = (other.transform.position - transform.position).normalized;

                // 施加推力
                playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Force);
                isPushed = false;
            }
            yield break;
        }

    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController control = other.GetComponent<PlayerController>();

        if (control != null && control.CompareTag("Player"))
        {
            isPushed = false;
            animator.SetBool("playerOn", false); // 设置动画状态为 false
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); // 停止協程
                currentCoroutine = null;
            }
        }
    }
}