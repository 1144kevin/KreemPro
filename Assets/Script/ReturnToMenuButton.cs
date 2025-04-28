using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
  [SerializeField] private SceneAudioSetter sceneAudioSetter;
  public void OnClickReturn()
  {
    sceneAudioSetter?.PlayConfirmSound();
    SceneManager.LoadScene("main menu");
  }
}