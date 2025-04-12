using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public override void Spawned()
    {
        Debug.Log($"✅ PlayerSpawner Spawned on {Object.InputAuthority} / StateAuthority: {Object.HasStateAuthority}");

        if (!Object.HasStateAuthority)
            return;

        var gameManager = GameManager.Instance;

        foreach (var playerData in gameManager.PlayerList.Values)
        {
            var spawnPoint = SpawnPosition.GetSpawnPosition(playerData.Object.InputAuthority);
            var characterPrefab = gameManager.CharacterPrefabs[playerData.SelectedCharacterIndex];
            var playerObj = Runner.Spawn(characterPrefab, spawnPoint, Quaternion.identity, playerData.Object.InputAuthority);
            Runner.SetPlayerObject(playerData.Object.InputAuthority, playerObj); // ⬅️ 加上這行
        }
    }
}