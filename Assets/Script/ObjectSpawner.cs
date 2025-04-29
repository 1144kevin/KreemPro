using System.Collections.Generic;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef shotPrefab;

    private readonly HashSet<NetworkObject> spawnedObjects = new HashSet<NetworkObject>();

    public void SpawnKreem()//之後寫生成Kreem可以直接用
    {
        if (Runner.IsClient) return;

        var obj = Runner.Spawn(shotPrefab, transform.position + Random.insideUnitSphere * 3);
        spawnedObjects.Add(obj);

    }

    public Shot SpawnShot(Vector3 position, Quaternion rotation)
    {
        if (Runner.IsClient) return null;

        var obj = Runner.Spawn(shotPrefab, position, rotation);
        //spawnedObjects.Add(obj);
        return obj.GetComponent<Shot>();
    }

    // public void DespawnShot(NetworkObject shot)
    // { 
    //     Debug.Log("despawn");
    //     if (shot == null) return;

    //     if (spawnedObjects.Contains(shot))
    //     {
           
    //         // spawnedObjects.Remove(shot);
    //         spawnedObjects.Clear();
    //     }

    //     Runner.Despawn(shot);
    // }

    // public void DespawnAll()
    // {
    //     if (Runner.IsClient) return; // Clients can't despawn.

    //     if (spawnedObjects == null) return;

    //     foreach (var obj in spawnedObjects)
    //     {
    //         Runner.Despawn(obj);
    //     }

    //     spawnedObjects.Clear();
    // }

}
