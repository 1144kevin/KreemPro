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
        if (Runner.IsClient)return null;
        
        var obj = Runner.Spawn(shotPrefab, position, rotation);
        spawnedObjects.Add(obj);
        return obj.GetComponent<Shot>();
    }

    public void DespawnShot(){
        
    }

}
