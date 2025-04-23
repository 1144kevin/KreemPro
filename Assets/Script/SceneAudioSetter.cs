using UnityEngine;

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
        if (confirmSFX != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(startSFX);
        }
    }
}
