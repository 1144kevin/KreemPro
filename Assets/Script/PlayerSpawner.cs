using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawner : NetworkBehaviour
{
    public override void Spawned()
    {
        Debug.Log("📦 PlayerSpawner Spawned 被執行");
        Debug.Log($"✅ PlayerSpawner Spawned on {Object.InputAuthority} / StateAuthority: {Object.HasStateAuthority}");

        if (!Object.HasStateAuthority)
            return;

        var gameManager = GameManager.Instance;

        foreach (var playerData in gameManager.PlayerList.Values)
        {
            // 🔧 僅在 FinalScene 執行 Spawn，避免在 Loading Scene 出現
            if (!SceneManager.GetActiveScene().name.Equals("FinalScene"))
                return;

            var spawnPoint = SpawnPosition.GetSpawnPosition(playerData.Object.InputAuthority);
            var characterPrefab = gameManager.CharacterPrefabs[playerData.SelectedCharacterIndex];

            var playerObj = Runner.Spawn(characterPrefab, spawnPoint, Quaternion.identity, playerData.Object.InputAuthority);
            Runner.SetPlayerObject(playerData.Object.InputAuthority, playerObj);

            Debug.Log($"👤 Spawned player {playerData.Object.InputAuthority} at {spawnPoint}");
        }
    }
}
