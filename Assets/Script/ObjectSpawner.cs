using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectSpawner : NetworkBehaviour
{
    [SerializeField] private NetworkPrefabRef _spherePrefab;
    [SerializeField] private NetworkPrefabRef _cubePrefab;

    private readonly HashSet<NetworkObject> _spawnedObjects = new HashSet<NetworkObject>();

    public void SpawnSphere()
    {
        if (Runner.IsClient) return; // Clients can't spawn.
        
        var obj = Runner.Spawn(_spherePrefab, transform.position + Random.insideUnitSphere * 3);
        _spawnedObjects.Add(obj);
    }
    
    public void SpawnCube()
    {
        if (Runner.IsClient) return; // Clients can't spawn.
        
        var obj = Runner.Spawn(_cubePrefab, transform.position + Random.insideUnitSphere * 3);
        _spawnedObjects.Add(obj);
    }

    public void DespawnAll()
    {
        if (Runner.IsClient) return; // Clients can't despawn.
        
        if (_spawnedObjects == null) return;

        foreach (var networkObject in _spawnedObjects)
        {
            Runner.Despawn(networkObject);
        }
        
        _spawnedObjects.Clear();
    }
}
