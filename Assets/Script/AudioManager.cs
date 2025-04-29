using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance { get; private set; }

  [Header("Mixer & Source")]
  [SerializeField] private AudioMixerGroup bgmMixerGroup;
[SerializeField] private AudioMixerGroup sfxMixerGroup;
  [SerializeField] private AudioMixer mainMixer;
  [SerializeField] private AudioSource bgmSource;
  [SerializeField] private AudioSource sfxSource;
  private AudioSource loopingSfxSource;

  void Awake()
  {
    bgmSource.outputAudioMixerGroup = bgmMixerGroup;
    sfxSource.outputAudioMixerGroup = sfxMixerGroup;
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    DontDestroyOnLoad(gameObject);
    SceneManager.sceneUnloaded += OnSceneUnloaded;
    SceneManager.sceneLoaded += OnSceneLoaded;
  }

  private void OnDestroy()
  {
    SceneManager.sceneUnloaded -= OnSceneUnloaded;
    SceneManager.sceneLoaded -= OnSceneLoaded;
  }

  private void OnSceneUnloaded(Scene current)
  {
    StopLoopingSFX();
  }

  private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
  {
    StopBGM(); // ✅ 每次進入新場景先停止舊 BGM
  }

  /* ---------- Public API ---------- */
  public void PlayBGM(AudioClip clip, float vol = .7f, bool loop = true)
  {
    if (clip == null) return;
    bgmSource.clip = clip;
    bgmSource.volume = vol;
    bgmSource.loop = loop;
    bgmSource.Play();
  }

  public void StopBGM()
  {
    if (bgmSource.isPlaying)
    {
      bgmSource.Stop();
      bgmSource.clip = null;
    }
  }

  public void PlaySFX(AudioClip clip, float vol = 1f)
  {
    if (clip == null) return;
    sfxSource.PlayOneShot(clip, vol);
  }

  public void PlayLoopingSFX(AudioClip clip, float vol = 1f)
  {
    if (clip == null) return;

    if (loopingSfxSource == null)
    {
      GameObject sfxLoopObj = new GameObject("LoopingSFX");
      loopingSfxSource = sfxLoopObj.AddComponent<AudioSource>();
      DontDestroyOnLoad(sfxLoopObj);
    }

    loopingSfxSource.clip = clip;
    loopingSfxSource.volume = vol;
    loopingSfxSource.loop = true;
    loopingSfxSource.Play();
  }

  public void StopLoopingSFX()
  {
    if (loopingSfxSource != null)
    {
      loopingSfxSource.Stop();
      Destroy(loopingSfxSource.gameObject);
      loopingSfxSource = null;
    }
  }

  /* 3D 版：呼叫後在世界座標播放一次 */
  public void PlaySFXAtPos(AudioClip clip, Vector3 pos, float vol = 1f)
  {
    if (clip == null) return;
    AudioSource.PlayClipAtPoint(clip, pos, vol);
  }
}
