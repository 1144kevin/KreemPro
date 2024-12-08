using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private int damage = 10; // 将damage改为int并赋值默认值
    [SerializeField] private Animator animator;
    private bool isPushed = false;

    private void Start()
    {
        isPushed = false;
    }
    private void OnTriggerEnter(Collider other)
    {
        PlayerController control = other.GetComponent<PlayerController>();
        Health damage=other.GetComponent<Health>();

        if (!isPushed)
        {
            if (control != null && control.CompareTag("Player"))
            {
                isPushed = true;
                Debug.Log($"{other.name} 踩到");
                animator.SetBool("playerOn", true); // 设置动画状态为 true
                damage.TakeDamage(20);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        PlayerController control = other.GetComponent<PlayerController>();

        if (control != null && control.CompareTag("Player"))
        {
            isPushed = false;
            animator.SetBool("playerOn", false); // 设置动画状态为 false
        }
    }
}