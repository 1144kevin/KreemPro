using UnityEngine;
using Fusion;

public static class SpawnPosition
{
    private static readonly Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(2500f, 50f, 120f),
        new Vector3(2500f, 50f, -2500f),
        new Vector3(-150f, 50f, 120f),
        new Vector3(-150f, 50f, -2500f)
    };

    public static Vector3 GetSpawnPosition(PlayerRef player)
    {
        int index = (int)(player.PlayerId - 1) % spawnPositions.Length;
        return spawnPositions[index];
    }
}