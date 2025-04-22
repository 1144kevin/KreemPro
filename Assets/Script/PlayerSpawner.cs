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

        if (!SceneManager.GetActiveScene().name.Equals("FinalScene"))
            return;

        foreach (var playerData in gameManager.PlayerList.Values)
        {
            var spawnPoint = SpawnPosition.GetSpawnPosition(playerData.Object.InputAuthority);
            var characterPrefab = gameManager.CharacterPrefabs[playerData.SelectedCharacterIndex];

            var playerObj = Runner.Spawn(characterPrefab, spawnPoint, Quaternion.identity, playerData.Object.InputAuthority);
            Runner.SetPlayerObject(playerData.Object.InputAuthority, playerObj);

           var attackHandler = playerObj.GetComponentInChildren<AttackHandler2>();
           
            if (attackHandler != null)
            {
                var spawner = FindObjectOfType<ObjectSpawner>();
                if (spawner != null)
                {
                    Debug.Log($"[Spawn時] 建立 AttackHandler2 on {attackHandler.gameObject.name} | ID: {attackHandler.GetInstanceID()}");
                    attackHandler.SetSpawner(spawner);
                }
                else
                {
                    Debug.LogError("❌ 找不到 ObjectSpawner！");
                }
            }
            else
            {
                Debug.LogError("❌ 角色預製體上找不到 AttackHandler 組件！");
            }

            Debug.Log($"👤 Spawned player {playerData.Object.InputAuthority} at {spawnPoint}");
        }
    }
}
