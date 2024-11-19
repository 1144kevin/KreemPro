using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class SpikeTrap : MonoBehaviour
{
    public List<PlayerController> ListCharacters = new List<PlayerController>();
    public List<Spike> ListSpikes = new List<Spike>();
    [SerializeField] private int damage = 10;
    [SerializeField] private Animator animator;

    Coroutine SpikeTriggerRoutine;
    bool SpikesReload;

    private void Start()
    {
        SpikeTriggerRoutine = null;
        SpikesReload = true;
        ListCharacters.Clear();
        ListSpikes.Clear();

        // Spike[] arr = this.gameObject.GetComponentsInChildren<Spike>();
        // foreach (Spike s in arr)
        // {
        //     ListSpikes.Add(s);
        // }

    }

    private void Update()
    {
        if (ListCharacters.Count != 0)
        {
            foreach (PlayerController control in ListCharacters)
            {
                if (SpikeTriggerRoutine == null && SpikesReload == true)
                {
                    SpikeTriggerRoutine = StartCoroutine(_TriggerSpikes(control));
                }
            }
        }
    }

    IEnumerator _TriggerSpikes(PlayerController control)
    {
        // SpikesReload = false;
        // Debug.Log("spike triggered");

        // for (int i = 0; i < ListSpikes.Count; i++)// foreach (Spike s in ListSpikes)//spike升起
        // {
        //     Debug.Log("spike triggered");
        //     Spike s = ListSpikes[i];
        //     s.Shoot();
        // }
        if (control.CompareTag("Player"))//扣血
        {
            Health playerHealth = control.GetComponent<Health>();
            if (playerHealth != null) // 检查是否找到Health组件
            {
                playerHealth.TakeDamage(damage);

            }
        }
        yield return new WaitForSeconds(1f);
        // for (int i = 0; i < ListSpikes.Count; i++)//spike降下
        // {
        //     Spike s = ListSpikes[i];
        //     s.Retract();
        // }
        yield return new WaitForSeconds(1f);

        // SpikeTriggerRoutine = null;
        // SpikesReload = true;
    }

    public static bool IsTrap(GameObject obj)//這邊他有在一個地方改了一個東西不知道可不可行
    {
        if (obj.transform.root.gameObject.GetComponent<SpikeTrap>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerController control = other.gameObject.transform.root.gameObject.GetComponent<PlayerController>();

        if (control != null)
        {
            if (!ListCharacters.Contains(control))
            {
                ListCharacters.Add(control);
            }
        }
        if (control.CompareTag("Player"))
        {
            animator.SetBool("TrapTrigger", true);

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
//     if(control.damageDetector.DamageTaken==0)
//     {
//         if(SpikeTriggerRoutine==null&&SpikesReload==true)
//         {
//             SpikeTriggerRoutine=StartCoroutine(_TriggerSpikes());
//         }

//     }
// }


// public class SpikeTrap : MonoBehaviour
// {
//     [SerializeField] private int damage = 10; // 将damage改为int并赋值默认值
//      [SerializeField] private float yOffset = 1.0f; // Y軸位移的數值，可在Inspector中調整

//      [SerializeField] private GameObject spike;


//     private void OnTriggerEnter(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             Health playerHealth = other.GetComponent<Health>();
//             if (playerHealth != null) // 检查是否找到Health组件
//             {
//                 playerHealth.TakeDamage(damage);
//             }
//         }
//                 // 修改 spike 的 Y 軸位置
//             if (spike != null)
//             {
//                 Vector3 newPosition = spike.transform.position; // 獲取當前位置
//                 newPosition.y += yOffset; // 增加 Y 軸位移
//                 spike.transform.position = newPosition; // 設置新位置
//             }
//             else
//             {
//                 Debug.LogWarning("Spike GameObject is not assigned!");
//             }
//     }
// }
