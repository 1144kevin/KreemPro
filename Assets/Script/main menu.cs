using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class mainmenu : MonoBehaviour
{
   [SerializeField] private SceneAudioSetter sceneAudioSetter;
   [SerializeField]
   private Button startBtn;
   public void Start()
   {
      EventSystem.current.SetSelectedGameObject(startBtn.gameObject);
   }
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

