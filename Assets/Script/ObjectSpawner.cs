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
        Debug.Log("shot1");
        
        var obj =Runner.Spawn(shotPrefab, transform.position + Random.insideUnitSphere * 3);
        spawnedObjects.Add(obj);
        // if (obj == null)
        // {
        //     obj = Runner.Spawn(shotPrefab, transform.position + Random.insideUnitSphere * 3);
        //     spawnedObjects.Add(obj);
        // }
        // else
        // {
        //     obj.gameObject.SetActive(true);
        //     spawnedObjects.Add(obj);
        // }
    }

    public void DespawnAll()
    {
        if (Runner.IsClient) return; // Clients can't despawn.
        
        if (spawnedObjects == null) return;

        foreach (var obj in spawnedObjects)
        {
            Runner.Despawn(obj);
        }
        Debug.Log("despawn");
        spawnedObjects.Clear();
    }
}
