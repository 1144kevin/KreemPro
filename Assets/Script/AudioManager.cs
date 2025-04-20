using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
  public static AudioManager Instance { get; private set; }

  [Header("Mixer & Source")]
  [SerializeField] private AudioMixer mainMixer;
  [SerializeField] private AudioSource bgmSource;
  [SerializeField] private AudioSource sfxSource;

  [Header("Clips")]
  public AudioClip uiClick;
  public AudioClip damage;
  public AudioClip respawnInput;
  public AudioClip respawnSuccess;

  void Awake()
  {
    if (Instance != null)
    {
      Destroy(gameObject);
      return;
    }
    Instance = this;
    // 這一行就會讓這個 GameObject 進入新場景後也不被銷毀
    DontDestroyOnLoad(gameObject);
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

  public void PlaySFX(AudioClip clip, float vol = 1f)
  {
    if (clip == null) return;
    sfxSource.PlayOneShot(clip, vol);
  }

  /* 3D 版：呼叫後在世界座標播放一次 */
  public void PlaySFXAtPos(AudioClip clip, Vector3 pos, float vol = 1f)
  {
    if (clip == null) return;
    AudioSource.PlayClipAtPoint(clip, pos, vol);
  }
}
