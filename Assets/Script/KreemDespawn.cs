using Fusion;
using UnityEngine;

public class KreemDespawn : NetworkBehaviour
{
    private bool alreadyCollected = false;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;

    private void OnTriggerEnter(Collider other)
    {
        
        if (alreadyCollected || !Runner.IsServer) return;

        if (other.CompareTag("Player"))
        {
            sceneAudioSetter?.PlayKreemSound();
            var player = other.GetComponent<Player>();
            if (player == null) return;
            
                player.ServerAddKreem(); // 👈 Server 端加分
                alreadyCollected = true;
                Runner.Despawn(Object);  // 👈 伺服器統一消除 Kreem
        }
    }
}
