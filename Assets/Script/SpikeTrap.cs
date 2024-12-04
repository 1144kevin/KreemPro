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
    // public ParticleSystem HittedEffect;
    private void Start()
    {
        isPushed = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController control = other.GetComponent<PlayerController>();

        if (isPushed == false)
        {
            if (control != null && control.CompareTag("Player"))
            {
                isPushed = true;
                animator.SetBool("TrapTrigger", true);
                StartCoroutine(TriggerTrap(other));
                Debug.Log("TriggerTrap");
            }

        }

    }
    private IEnumerator TriggerTrap(Collider other)
    {
        // yield return new WaitForSeconds(0.5f);
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
        PlayerController control = other.gameObject.transform.root.gameObject.GetComponent<PlayerController>();

        if (control != null)
        {
            if (ListCharacters.Contains(control))
            {
                ListCharacters.Remove(control);
            }
        }
    }
}