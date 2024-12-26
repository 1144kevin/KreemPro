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

    [SerializeField] private GameObject[] charactersingame;
    [SerializeField] private GameObject[] charactersingame1;
    [SerializeField] private GameObject[] charactersingame2;
    [SerializeField] private GameObject[] charactersingame3;

    // 用於記錄角色是否已被選擇
    private static bool[] isCharacterSelected = new bool[4]; // 假設角色列表有4個角色
    private int selectedCharacterIndex = -1;

    [SerializeField] private Camera selectioncamera; 
     [SerializeField] private CameraFollower cameraFollower;
     [SerializeField] private CameraFollower1 cameraFollower1;
     [SerializeField] private CameraFollower2 cameraFollower2;
     [SerializeField] private CameraFollower3 cameraFollower3;
     [SerializeField] public GameObject canvas;

    private void Start()
    {
        if (canvas != null)
        {
            canvas.SetActive(false); // 隱藏 Canvas 初始狀態
        }
    }
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

        for (int i = 0; i < charactersingame.Length; i++)
        {
            charactersingame[i].SetActive(false);
        }
        charactersingame[index].SetActive(true);

        if (cameraFollower != null && charactersingame[index] != null) {
            cameraFollower.SetTarget(charactersingame[index].transform);
        }

        selectedCharacterIndex = index;
        
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

        for (int i = 0; i < charactersingame1.Length; i++)
        {
            charactersingame1[i].SetActive(false);
        }
        charactersingame1[index].SetActive(true);

        if (cameraFollower1 != null && charactersingame1[index] != null) {
            cameraFollower1.SetTarget(charactersingame1[index].transform);
        }


        selectedCharacterIndex = index;
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

        for (int i = 0; i < charactersingame2.Length; i++)
        {
            charactersingame2[i].SetActive(false);
        }

        charactersingame2[index].SetActive(true);
        if (cameraFollower2 != null && charactersingame2[index] != null) {
            cameraFollower2.SetTarget(charactersingame2[index].transform);
        }


        selectedCharacterIndex = index;
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

        for (int i = 0; i < charactersingame3.Length; i++)
        {
            charactersingame3[i].SetActive(false);
        }
        charactersingame3[index].SetActive(true);

        if (cameraFollower3 != null && charactersingame3[index] != null) {
            cameraFollower3.SetTarget(charactersingame3[index].transform);
        }


        selectedCharacterIndex = index;
    }

// Method to hide the camera when the "Go" button is pressed
    public void OnGoButtonPressed()
    {
        if (selectioncamera != null)
        {
            selectioncamera.gameObject.SetActive(false); // 隱藏攝影機 
        }
        if (canvas != null)
        {
            canvas.SetActive(true); // 顯示 Canvas
        }
    }  
}   