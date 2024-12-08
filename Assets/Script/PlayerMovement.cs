using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float Speed = 5f;
    public float GroundDistance = 0.2f;
    public LayerMask Ground;
    public float customGravity = 30.0f;
    private Animator _anim;
    private Rigidbody _body;
    private Vector3 _inputs = Vector3.zero;
    private bool _isGrounded = true;
    private Transform _groundChecker;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        _body = GetComponent<Rigidbody>();
        _groundChecker = transform.GetChild(0);
        _anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 自定義重力
        _body.AddForce(Vector3.down * customGravity, ForceMode.Acceleration);

        if (_isGrounded == Physics.CheckSphere(_groundChecker.position, GroundDistance, Ground, QueryTriggerInteraction.Ignore))
        {
            print("Hit");
        }

        // Reset movement input
        _inputs = Vector3.zero;

        // Use specific keys for movement
        if (Input.GetKey(KeyCode.RightArrow))
        {
            _inputs.x = -1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            _inputs.x = 1;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            _inputs.z = 1;
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            _inputs.z = -1;
        }

        if (_inputs != Vector3.zero)
        {
            _body.MovePosition(_body.position + _inputs * Speed * Time.fixedDeltaTime);
            transform.forward = _inputs;
            _anim.SetBool("Run", true);
        }
        else
        {
            _anim.SetBool("Run", false);
        }

        if (Input.GetKey(KeyCode.P)) // 確保攻擊動畫只觸發一次
        {
            animator.SetTrigger("isRunAttack");
            animator.SetTrigger("isAttack");
        }
        else// 當按鍵釋放時重置狀態
        {
            animator.SetTrigger("unAttack");
            animator.SetTrigger("unRunAttack");
        }
    }
}
