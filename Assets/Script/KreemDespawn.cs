using Fusion;
using UnityEngine;

public class KreemDespawn : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"collided with {other.name}. Despawning Kreem.");
            // 只有 State Authority 可以呼叫 despawn
            if (Object.HasStateAuthority)
            {
                Runner.Despawn(Object);
            }
        }
    }
}
