using Fusion;
using UnityEngine;

public class PlayerAudio : NetworkBehaviour
{
    [SerializeField] private SceneAudioSetter sceneAudioSetter;

    private NetworkObject netObj;

    private void Awake()
    {
        netObj = GetComponent<NetworkObject>();
        if (sceneAudioSetter == null)
        {
            sceneAudioSetter = FindObjectOfType<SceneAudioSetter>();
        }
    }

    public void PlayAttackSound(int characterSoundIndex)
    {
        if (!netObj.HasInputAuthority) return;

        var clip = sceneAudioSetter?.GetAttackSFXByCharacterIndex(characterSoundIndex);
        if (clip != null)
        {
            AudioManager.Instance?.PlaySFX(clip);
        }
    }

    public void PlayKreemSound()
    {
        if (!netObj.HasInputAuthority) return;

        var clip = sceneAudioSetter?.kreemSFX;
        if (clip != null)
        {
            AudioManager.Instance?.PlaySFX(clip);
        }
    }

    public void PlayDieSound()
    {
        if (!netObj.HasInputAuthority) return;

        sceneAudioSetter?.PlayDieSound();
    }

    public void PlayRebornSound()
    {
        if (!netObj.HasInputAuthority) return;

        sceneAudioSetter?.PlayRebornSound();
    }

    // public void PlayHitSound()
    // {
    //     if (!netObj.HasInputAuthority) return;

    //     var clip = sceneAudioSetter?.hitSFX;
    //     if (clip != null)
    //     {
    //         AudioManager.Instance?.PlaySFX(clip);
    //     }
    // }
}
