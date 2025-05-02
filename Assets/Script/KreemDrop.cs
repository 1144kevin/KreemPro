using UnityEngine;



public class KreemDrop : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
{

     Debug.Log($"觸發了：{other.name}");
     
    if (other.gameObject.CompareTag("Ground"))
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}

}
