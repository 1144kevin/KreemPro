using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10; // 将damage改为int并赋值默认值
    public List<PlayerController> ListCharacters = new List<PlayerController>();
    // public List<Spike> ListSpikes = new List<Spike>();
    [SerializeField] private int damage = 10;
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
        SpikeTriggerRoutine = null;
        SpikesReload = true;
        ListCharacters.Clear();
        // ListSpikes.Clear();

        // Spike[] arr = this.gameObject.GetComponentsInChildren<Spike>();
        // foreach (Spike s in arr)
        // {
        //     ListCharacters.Add(control);
        // }
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

    // void OnCollisionEnter(Collision collision)
    // {
    //     // 檢查碰撞對象是否是玩家
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         Debug.Log("Player hit!");

    //         // 獲取玩家的 Rigidbody
    //         Rigidbody playerRigidbody = collision.gameObject.GetComponent<Rigidbody>();

    //         if (playerRigidbody != null)
    //         {
    //             // 計算推動方向（從碰撞點推開玩家）
    //             Vector3 pushDirection = collision.contacts[0].point - transform.position;
    //             pushDirection = -pushDirection.normalized; // 反向推動方向，使玩家遠離

    //             // 施加推力
    //             float pushForce = 10f; // 可調整的推力大小
    //             playerRigidbody.AddForce(pushDirection * pushForce, ForceMode.Impulse);
    //         }
    //     }
    // }

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
        if (control.CompareTag("Player"))
        {
            animator.SetBool("TrapTrigger", false);

        }

    }
}
// private void OnTriggerEnter(Collider other)
// {

//     PlayerController control = other.gameObject.transform.root.gameObject.GetComponent<PlayerController>();

//     if (control != null)
//     {
//         if (!ListCharacters.Contains(control))
//         {
//             ListCharacters.Add(control);
//         }
//     }
//     if (control.CompareTag("Player"))
//     {
//         animator.SetBool("TrapTrigger", true);

//     }       

//      yield return new WaitForSeconds(1f);

//      if (other.CompareTag("Player"))
//     {
//         Health playerHealth = other.GetComponent<Health>();
//         if (playerHealth != null) // 检查是否找到Health组件
//         {
//             playerHealth.TakeDamage(damage);
//         }
//     }



// }
