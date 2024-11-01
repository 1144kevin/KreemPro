using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class characterselect : MonoBehaviour
{
    private int selectedcharacterIndex;

    [Header("List of characters")]
    [SerializeField] private List<characterselectObject> characterList = new List<characterselectObject>();

    [Header("UI References")]
    [SerializeField]private TextMeshProUGUI characterName;
    [SerializeField]private Image characterSplash;


    [Header("Sounds")]
    [SerializeField]private AudioClip arrowClickSFX;
    [SerializeField]private AudioClip characterSelectMusic;


    private void Start()
    {
       UpdateCharacterSelectionUI();
    }

    public void leftArrow()
    {
        selectedcharacterIndex--;
        if(selectedcharacterIndex<0)
           selectedcharacterIndex=characterList.Count-1;

           UpdateCharacterSelectionUI();
    }
    public void RightArrow()
    {
        selectedcharacterIndex++;
        if(selectedcharacterIndex==characterList.Count)
           selectedcharacterIndex=0;

           UpdateCharacterSelectionUI();
    }

    public void Confirm()
    {
        Debug.Log(string.Format("Character{0}:{1} has been chosen",selectedcharacterIndex,characterList[selectedcharacterIndex].name));
    }

     private void UpdateCharacterSelectionUI()
     {
      characterSplash.sprite=characterList[selectedcharacterIndex].splash;
      characterName.text=characterList[selectedcharacterIndex].name;
     }

     [System.Serializable]
     public class characterselectObject
     {
        public Sprite splash;
        public string name;


     }
}
