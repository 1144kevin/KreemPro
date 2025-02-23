using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHandler : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool attackInput = false;
    private bool isAttacking = false;

    public void PlayerAnimation(Vector3 input)
    {
        var isMoving = input.magnitude > 0.1f;

        if (isMoving)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
        // //攻擊
        // if (attackInput && !isAttacking) // 確保攻擊動畫只觸發一次
        // {
        //     isAttacking = true;

        //     if (isMoving)
        //     {
        //         animator.SetTrigger("isRunAttack");
        //     }
        //     else
        //     {
        //         animator.SetTrigger("isAttack");
        //     }
        // }
        // else if (!attackInput) // 當按鍵釋放時重置狀態
        // {
        //     isAttacking = false;
        //     animator.SetTrigger("unAttack");
        //     animator.SetTrigger("unRunAttack");
        // }

    }

}
