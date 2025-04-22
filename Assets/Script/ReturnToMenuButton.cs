using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToMenuButton : MonoBehaviour
{
  public void OnClickReturn()
  {
    SceneManager.LoadScene("main menu");
  }
}