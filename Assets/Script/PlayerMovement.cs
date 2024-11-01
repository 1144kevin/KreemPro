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

    void Start()
    {
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
        if (Input.GetKey(KeyCode.D))
        {
            _inputs.x = -1;
        }
        if (Input.GetKey(KeyCode.A))
        {
            _inputs.x = 1;
        }
        if (Input.GetKey(KeyCode.S))
        {
            _inputs.z = 1;
        }
        if (Input.GetKey(KeyCode.W))
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

        // Uncomment if jump or dash functions are needed
        /*
        if (Input.GetButtonDown("Jump") && _isGrounded)
        {
            _body.AddForce(Vector3.up * Mathf.Sqrt(JumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
        }
        if (Input.GetButton("Dash"))
        {
            Vector3 dashVelocity = Vector3.Scale(transform.forward, DashDistance * new Vector3(Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime, 0, Mathf.Log(1f / (Time.deltaTime * _body.drag + 1)) / -Time.deltaTime));
            _body.AddForce(dashVelocity, ForceMode.VelocityChange);
        }
        */
    }
}
