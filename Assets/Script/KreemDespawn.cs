using Fusion;
using UnityEngine;

public class KreemDespawn : NetworkBehaviour
{
    private bool alreadyCollected = false;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;
private void OnTriggerEnter(Collider other)
{
    if (alreadyCollected) return;

    if (other.CompareTag("Player"))
    {
        var player = other.GetComponent<Player>();
        if (player == null) return;

        if (Runner.IsServer)
        {
            player.playerAudio?.PlayKreemSound();  // ✅ 改用模組化播放音效
            player.ServerAddKreem();     // Server 端加分
            alreadyCollected = true;
            Runner.Despawn(Object);      // Server 端移除 Kreem
        }
    }
}

}
