using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    public List<PlayerController> ListCharacters = new List<PlayerController>();

    [SerializeField] private Animator animator;
    [SerializeField] private float pushForce = 5f;

    public float pushDuration = 2f;
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
            if (control != null && (collision.collider.CompareTag("Robot") || collision.collider.CompareTag("Mushroom") || collision.collider.CompareTag("Leopard") || collision.collider.CompareTag("Eagle")))
            {
                isPushed = true;
                animator.SetBool("playerOn", true);
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
            Rigidbody playerRigidbody = collision.collider.GetComponent<Rigidbody>();
            if (playerRigidbody != null)
            {
                Vector3 pushDirection = (collision.collider.transform.position - transform.position).normalized;
                playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Force);
                isPushed = false;
            }
            yield break;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        PlayerController control = collision.collider.GetComponent<PlayerController>();

        if (control != null && (collision.collider.CompareTag("Robot") || collision.collider.CompareTag("Mushroom") || collision.collider.CompareTag("Leopard") || collision.collider.CompareTag("Eagle")))
        {
            isPushed = false;
            animator.SetBool("playerOn", false);

        }
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }
    }
}
