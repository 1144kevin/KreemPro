using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef shotPrefab;

    private readonly HashSet<NetworkObject> spawnedObjects = new HashSet<NetworkObject>();

    public void SpawnSphere()
    {
        if (Runner.IsClient) return; // 確保只有伺服器能夠生成物件

        var obj = Runner.Spawn(shotPrefab, transform.position + Random.insideUnitSphere * 3);
        spawnedObjects.Add(obj);

    }

   public Shot SpawnShot(Vector3 position, Quaternion rotation)
    {
        if (Runner.IsClient)return null;
        
        var obj = Runner.Spawn(shotPrefab, position, rotation);
        return obj.GetComponent<Shot>();
    }

    public void DespawnAll()
    {
        if (Runner.IsClient) return; // Clients can't despawn.

        if (spawnedObjects == null) return;

        foreach (var obj in spawnedObjects)
        {
            Runner.Despawn(obj);
        }

        spawnedObjects.Clear();
    }
}
