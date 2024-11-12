/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class characterselect : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject[] characters1;
    [SerializeField] private GameObject[] characters2;
    [SerializeField] private GameObject[] characters3;


    [SerializeField] private GameObject[] charactersname;
    [SerializeField] private GameObject[] charactersname1;
    [SerializeField] private GameObject[] charactersname2;
    [SerializeField] private GameObject[] charactersname3;

    [Header("Sounds")]
    [SerializeField]private AudioClip arrowClickSFX;
    [SerializeField]private AudioClip characterSelectMusic;

     
   public void changecharacter(int index)
   {
      for(int i=0;i< characters.Length;i++)
       {
           characters[i].SetActive(false);
           charactersname[i].SetActive(false);
       }
       characters[index].SetActive(true);
       charactersname[index].SetActive(true);
   }
   
   public void changecharacter1(int index)
   {
      for(int i=0;i< characters1.Length;i++)
       {
           characters1[i].SetActive(false);
           charactersname1[i].SetActive(false);
       }
       characters1[index].SetActive(true);
       charactersname1[index].SetActive(true);
   }

   public void changecharacter2(int index)
   {
      for(int i=0;i< characters2.Length;i++)
       {
           characters2[i].SetActive(false);
           charactersname2[i].SetActive(false);
       }
       characters2[index].SetActive(true);
       charactersname2[index].SetActive(true);
   }

   public void changecharacter3(int index)
   {
      for(int i=0;i< characters3.Length;i++)
       {
           characters3[i].SetActive(false);
           charactersname3[i].SetActive(false);
       }
       characters3[index].SetActive(true);
       charactersname3[index].SetActive(true);
   }
   
}
*/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class characterselect : MonoBehaviour
{
    [SerializeField] private GameObject[] characters;
    [SerializeField] private GameObject[] characters1;
    [SerializeField] private GameObject[] characters2;
    [SerializeField] private GameObject[] characters3;

    [SerializeField] private GameObject[] charactersname;
    [SerializeField] private GameObject[] charactersname1;
    [SerializeField] private GameObject[] charactersname2;
    [SerializeField] private GameObject[] charactersname3;

    [Header("Sounds")]
    [SerializeField] private AudioClip arrowClickSFX;
    [SerializeField] private AudioClip characterSelectMusic;

    // 用於記錄角色是否已被選擇
    private static bool[] isCharacterSelected = new bool[4]; // 假設角色列表有4個角色
    private int selectedCharacterIndex = -1;

    // 玩家 1 的選角
    public void changecharacter(int index)
    {
        if (isCharacterSelected[index])
        {
            Debug.Log("This character is already selected by another player!");
            return; // 如果角色已被選擇，返回並提示
        }

        for (int i = 0; i < characters.Length; i++)
        {
            characters[i].SetActive(false);
            charactersname[i].SetActive(false);
        }
        characters[index].SetActive(true);
        charactersname[index].SetActive(true);

        selectedCharacterIndex = index;
        AudioSource.PlayClipAtPoint(arrowClickSFX, transform.position);
    }

    // 玩家 2 的選角
    public void changecharacter1(int index)
    {
        if (isCharacterSelected[index])
        {
            Debug.Log("This character is already selected by another player!");
            return;
        }

        for (int i = 0; i < characters1.Length; i++)
        {
            characters1[i].SetActive(false);
            charactersname1[i].SetActive(false);
        }
        characters1[index].SetActive(true);
        charactersname1[index].SetActive(true);

        selectedCharacterIndex = index;
        AudioSource.PlayClipAtPoint(arrowClickSFX, transform.position);
    }

    // 玩家 3 的選角
    public void changecharacter2(int index)
    {
        if (isCharacterSelected[index])
        {
            Debug.Log("This character is already selected by another player!");
            return;
        }

        for (int i = 0; i < characters2.Length; i++)
        {
            characters2[i].SetActive(false);
            charactersname2[i].SetActive(false);
        }
        characters2[index].SetActive(true);
        charactersname2[index].SetActive(true);

        selectedCharacterIndex = index;
        AudioSource.PlayClipAtPoint(arrowClickSFX, transform.position);
    }

    // 玩家 4 的選角
    public void changecharacter3(int index)
    {
        if (isCharacterSelected[index])
        {
            Debug.Log("This character is already selected by another player!");
            return;
        }

        for (int i = 0; i < characters3.Length; i++)
        {
            characters3[i].SetActive(false);
            charactersname3[i].SetActive(false);
        }
        characters3[index].SetActive(true);
        charactersname3[index].SetActive(true);

        selectedCharacterIndex = index;
        AudioSource.PlayClipAtPoint(arrowClickSFX, transform.position);
    }

    // 確認選擇按鈕
    public void ConfirmSelection(int player)
    {
        if (selectedCharacterIndex == -1)
        {
            Debug.Log("No character selected to confirm!");
            return;
        }

        // 設定選擇狀態為已選
        isCharacterSelected[selectedCharacterIndex] = true;
        Debug.Log("Player " + player + " confirmed character " + selectedCharacterIndex);
        
        // 播放選擇確認音效
        AudioSource.PlayClipAtPoint(characterSelectMusic, transform.position);
    }
}