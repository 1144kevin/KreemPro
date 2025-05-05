using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class MelodySequence
{
    public AudioClip[] notes; //譜曲包裝容器
}

public class SceneAudioSetter : MonoBehaviour
{
    [Header("Scene BGM")]
    [SerializeField] private AudioClip sceneBGM;
    [SerializeField] private float bgmVolume = 0.7f;
    [SerializeField] private bool bgmLoop = true;

    [Header("Scene SFX (Optional)")]
    [SerializeField] private AudioClip sceneIntroSFX;
    [SerializeField] private float sfxVolume = 1.0f;
    [SerializeField] private bool playSFXOnStart = false;

    [Header("Looping SFX (Optional)")]
    [SerializeField] private AudioClip loopingSFX;
    [SerializeField] private float loopingSFXVolume = 1.0f;
    [SerializeField] private bool playLoopingSFXOnStart = false;

    [Header("UI/選角音效 (Optional)")]
    [SerializeField] private AudioClip hoverSFX;
    [SerializeField] private AudioClip confirmSFX;
    [SerializeField] private AudioClip startSFX;
    [SerializeField] private AudioClip dieSFX;
    [SerializeField] private AudioClip ringSFX;
    [SerializeField] private AudioClip rebornSFX;
    [SerializeField] private AudioClip TimeNotifySFX;
    [SerializeField] private AudioClip TenSecCountdownSFX;




    [Header("場上物件")]
    [SerializeField] public AudioClip kreemSFX;

    [Header("各角色攻擊 SFX（依角色 index）")]
    [SerializeField] private AudioClip[] characterAttackSFX;
    [SerializeField] private AudioClip RobotOneShotSFX;

    [Header("復活樂譜")]
    [SerializeField] private List<MelodySequence> melodySequences;
    [SerializeField] private AudioClip wrongSFX;

    [Header("聲音大小設定")]
    [SerializeField] public float melodyVolume = 1.0f; // 預設 1.0f，可拉高
    [SerializeField] public float rebornVolume = 1.0f;
    [SerializeField] public float TimerNotifyVolume = 1.0f;




    void Start()
    {
        if (AudioManager.Instance == null)
        {
            Debug.LogWarning("[SceneAudioSetter] 找不到 AudioManager 實例");
            return;
        }

        // 播放背景音樂
        if (sceneBGM != null)
        {
            AudioManager.Instance.PlayBGM(sceneBGM, bgmVolume, bgmLoop);
            Debug.Log($"[SceneAudioSetter] 播放 BGM：{sceneBGM.name}");
        }

        // 播放場景進場音效（如果啟用）
        if (playSFXOnStart && sceneIntroSFX != null)
        {
            AudioManager.Instance.PlaySFX(sceneIntroSFX, sfxVolume);
            Debug.Log($"[SceneAudioSetter] 播放場景進場 SFX：{sceneIntroSFX.name}");
        }

        // 播放持續 SFX（如環境聲）
        if (playLoopingSFXOnStart && loopingSFX != null)
        {
            AudioManager.Instance.PlayLoopingSFX(loopingSFX, loopingSFXVolume);
            Debug.Log($"[SceneAudioSetter] 播放持續 SFX：{loopingSFX.name}");
        }
    }
    public void PlayWrongSound()
    {
        if (wrongSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(wrongSFX);
        }
    }
    public void PlayTenSecCoundownSound()
    {
        if (TenSecCountdownSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(TenSecCountdownSFX);
        }
    }
    public void PlayTimeNotifySound()
    {
        if (TimeNotifySFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(TimeNotifySFX,TimerNotifyVolume);
        }
    }
    public void PlayRebornSound()
    {
        if (rebornSFX != null && AudioManager.Instance != null)
        {
            float safeVolume = Mathf.Clamp(rebornVolume, 0f, 1f);
            AudioManager.Instance.PlaySFX(rebornSFX, safeVolume);
        }
    }
    public void PlayKreemSound()
    {
        if (kreemSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(kreemSFX);
        }
    }
    public void PlayHoverSound()
    {
        if (hoverSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(hoverSFX);
        }
    }

    public void PlayConfirmSound()
    {
        if (confirmSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(confirmSFX);
        }
    }
    public void PlayStartSound()
    {
        if (startSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(startSFX);
        }
    }
    public void PlayRingSound()
    {
        if (ringSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(ringSFX);
        }
    }
    public void PlayDieSound()
    {
        if (dieSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(dieSFX);
        }
    }
    public void PlayOneShotSound()
    {
        if (RobotOneShotSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(RobotOneShotSFX);
        }
    }
    public int GetMelodySequenceCount()
    {
        return melodySequences.Count;
    }

    public AudioClip[] GetMelodySequenceByIndex(int index)
    {
        if (index < 0 || index >= melodySequences.Count) return null;
        return melodySequences[index].notes;
    }




    public AudioClip GetAttackSFXByCharacterIndex(int index)
    {
        if (index < 0 || index >= characterAttackSFX.Length) return null;
        return characterAttackSFX[index];
    }

}
