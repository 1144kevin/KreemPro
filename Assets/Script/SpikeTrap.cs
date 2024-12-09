using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10; // \u5c06damage\u6539\u4e3aint\u5e76\u8d4b\u503c\u9ed8\u8ba4\u503c
    public List<PlayerController> ListCharacters = new List<PlayerController>();

    [SerializeField] private Animator animator;
    [SerializeField] private float pushForce = 5f; // \u73a9\u5bb6\u88ab\u63a8\u7684\u529b\u91cf

    public float pushDuration = 2f; // \u6301\u7eed\u65f6\u95f42\u79d2
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    private Coroutine currentCoroutine;
    private Health playerHealth;

    void Start()
    {
        playerHealth = GetComponent<Health>();
        isPushed = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        PlayerController control = collision.collider.GetComponent<PlayerController>();

        if (!isPushed)
        {
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine);
            }
            if (control != null && collision.collider.CompareTag("Player"))
            {
                isPushed = true;
                animator.SetBool("playerOn", true);
                Debug.Log("ggggggggggg");

                // 在玩家身上取得 Health 元件
                playerHealth = collision.collider.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
                currentCoroutine = StartCoroutine(TriggerTrap(collision));
            }
        }
    }

    private IEnumerator TriggerTrap(Collision collision)
    {
        yield return new WaitForSeconds(1.0f);
        if (collision.collider.CompareTag("Player"))
        {
             playerHealth = collision.collider.GetComponent<Health>();
            // if (playerHealth != null)
            // {
            //     playerHealth.TakeDamage(damage);
            // }

            // \u7ed9\u73a9\u5bb6\u4e00\u4e2a\u63a8\u529b
            Rigidbody playerRigidbody = collision.collider.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                // \u8ba1\u7b97\u63a8\u529b\u65b9\u5411\uff08\u4f7f\u73a9\u5bb6\u8fdc\u79bb\u9677\u9631\u4e2d\u5fc3\uff09
                Vector3 pushDirection = (collision.collider.transform.position - transform.position).normalized;

                // \u65bd\u52a0\u63a8\u529b
                playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Force);
                isPushed = false;
            }
            yield break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerController control = collision.collider.GetComponent<PlayerController>();

        if (control != null && collision.collider.CompareTag("Player"))
        {
            isPushed = false;
            animator.SetBool("playerOn", false); // \u8bbe\u7f6e\u52a8\u753b\u72b6\u6001\u4e3a false
            if (currentCoroutine != null)
            {
                StopCoroutine(currentCoroutine); // \u505c\u6b62\u534f\u7ebf
                currentCoroutine = null;
            }
        }
    }
}
