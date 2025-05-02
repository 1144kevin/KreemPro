using System.Collections;
using UnityEngine;
using Fusion;

public class NetworkDespawnAfterDelayFForBrokenBox : NetworkBehaviour
{
    [SerializeField] private float delay = 3f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority) // 只有 Server 執行 Despawn
            StartCoroutine(DespawnLater());
    }

    private IEnumerator DespawnLater()
    {
        yield return new WaitForSeconds(delay);

        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}