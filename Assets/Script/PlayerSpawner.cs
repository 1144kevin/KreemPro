using Fusion;
using UnityEngine;

public class PlayerSpawner : NetworkBehaviour
{
    public override void Spawned()
    {
        if (!Object.HasStateAuthority)
            return;

        var gameManager = GameManager.Instance;

        foreach (var playerData in gameManager.PlayerList.Values)
        {
            var spawnPoint = new Vector3(0, 50, 0);
            var characterPrefab = gameManager.CharacterPrefabs[playerData.SelectedCharacterIndex];
            Runner.Spawn(characterPrefab, spawnPoint, Quaternion.identity, playerData.Object.InputAuthority);
        }
    }
}