using UnityEngine;

public class bulletHit : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision detected with: " + collision.gameObject.name);
        
        if (collision.gameObject.CompareTag("Target"))
        {
            Debug.Log("Hit target!");
            // Add any hit effects here
            Destroy(gameObject);
        }
    }
}