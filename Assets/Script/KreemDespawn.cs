using Fusion;
using UnityEngine;

public class KreemDespawn : NetworkBehaviour
{
    private bool alreadyCollected = false;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;
    private ObjectSpawner objectSpawner;
private void OnTriggerEnter(Collider other)
{
    if (alreadyCollected) return;

    if (other.CompareTag("Player"))
    {
        var player = other.GetComponent<Player>();
        if (player == null) return;

        if (Runner.IsServer)
        {
             player.RpcRequestPlayKreemSound(); // ★ 叫player自己觸發音效
            player.ServerAddKreem();     // Server 端加分
            alreadyCollected = true;
            Runner.Despawn(Object);      // Server 端移除 Kreem
        }
    }
}

}
