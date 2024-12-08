using UnityEngine;

public class PlayerHitted : MonoBehaviour
{
    public float pushForce;
    public float pushDuration;
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    public ParticleSystem HittedEffect;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // if (collision.gameObject.tag == "Trap" && !isPushed)
        // {
        //     Debug.Log("hit");
        //     rb.AddForce(Vector3.up * pushForce, ForceMode.Impulse);
        //     isPushed = true;
        //     HittedEffect.Play();
        //     pushEndTime = Time.time + pushDuration;
        // }
    }

    void Update()
    {
        if (isPushed && Time.time > pushEndTime)
        {
            StopPush();
        }
    }

    private void StopPush()
    {
        isPushed = false;
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
    }
}