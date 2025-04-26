using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class mainmenu : MonoBehaviour
{
   [SerializeField] private SceneAudioSetter sceneAudioSetter;
   public void Play()
   {
      
    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    sceneAudioSetter?.PlayConfirmSound();

   }

   public void Quit()
   {
    Application.Quit();
    sceneAudioSetter?.PlayConfirmSound();
    Debug.Log("player has exit the game");
   }
}

