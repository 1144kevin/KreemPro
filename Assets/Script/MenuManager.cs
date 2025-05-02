using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MenuManager : MonoBehaviour
{
    public enum MenuType
    {
        Menu,
        Room
    }
    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject roomPanel;

    private string playerName;

    // private string roomName;
    [SerializeField]
    private Button firstCharacterButton;
    [SerializeField] private GameObject characterPanel;
    [SerializeField] private GameObject createJoinPanel;

    [SerializeField]
    private Button createBtn;

    [SerializeField]
    private Button joinBtn;
    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private GameObject playerListContent;
    // [SerializeField] private TMP_Text characterName;
    [SerializeField] private GameObject[] characters3d;

    [SerializeField] private string[] charactersName;
    [SerializeField] private GameObject[] charactersNameImage;
    [SerializeField] private SceneAudioSetter sceneAudioSetter;

    private Button lastConfirmedButton;
    private Color lastButtonOriginalColor;

    [SerializeField] private GameObject startConfirmationPopup;
    [SerializeField] private Button yesButton;
    [SerializeField] private Button noButton;

    private void Start()
    {
        createBtn.onClick.AddListener(OnCreateBtnClicked);
        joinBtn.onClick.AddListener(OnJoinBtnClicked);
        startBtn.onClick.AddListener(OnStartBtnClicked);

        yesButton.onClick.AddListener(OnYesButtonClicked);
        noButton.onClick.AddListener(OnNoButtonClicked);

        EventSystem.current.SetSelectedGameObject(firstCharacterButton.gameObject);
    }
    private void OnDestroy()
    {
        createBtn.onClick.RemoveAllListeners();
        joinBtn.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();
    }

    private async void OnCreateBtnClicked()
    {
        sceneAudioSetter?.PlayConfirmSound();
        GameManager.Instance.PlayerName = playerName;
        GameManager.Instance.RoomName = "Kreem-11";
        createBtn.interactable = false;
        await GameManager.Instance.CreateRoom();
    }
    private async void OnJoinBtnClicked()
    {
        sceneAudioSetter?.PlayConfirmSound();
        GameManager.Instance.PlayerName = playerName;
        GameManager.Instance.RoomName = "Kreem-11";
        joinBtn.interactable = false;
        await GameManager.Instance.JoinRoom();
    }

    public void OnConfirmCharacterSelected()
    {
        GameObject selectedObj = EventSystem.current.currentSelectedGameObject;
        if (selectedObj == null)
        {
            Debug.Log("No selected object found!");
            return;
        }

        Button selectedBtn = selectedObj.GetComponent<Button>();
        if (selectedBtn == null)
        {
            Debug.Log("Selected object is not a Button!");
            return;
        }

        // 還原上一次按鈕的縮放（如果有）
        if (lastConfirmedButton != null)
        {
            lastConfirmedButton.transform.localScale = Vector3.one;
        }

        lastConfirmedButton = selectedBtn;
        ColorBlock cb = selectedBtn.colors;
        lastButtonOriginalColor = cb.normalColor;

        cb.normalColor = new Color32(139, 255, 218, 255);
        selectedBtn.colors = cb;

        // ✅ 放大目前選到的按鈕
        selectedBtn.transform.localScale = Vector3.one * 1.2f;

        EventSystem.current.SetSelectedGameObject(createBtn.gameObject);
        sceneAudioSetter?.PlayConfirmSound();

    }

    public void BackToCharacterSelection()
    {
        if (lastConfirmedButton != null)
        {
            // ✅ 還原顏色
            ColorBlock cb = lastConfirmedButton.colors;
            cb.normalColor = lastButtonOriginalColor;
            lastConfirmedButton.colors = cb;

            // ✅ 還原縮放
            lastConfirmedButton.transform.localScale = Vector3.one;
        }

        EventSystem.current.SetSelectedGameObject(firstCharacterButton.gameObject);
        sceneAudioSetter?.PlayHoverSound();
    }


    private void OnStartBtnClicked()
    {
        sceneAudioSetter?.PlayConfirmSound();
        startConfirmationPopup.SetActive(true);
        EventSystem.current.SetSelectedGameObject(yesButton.gameObject);
    }

    private void OnYesButtonClicked()
    {
        GameManager.Instance.StartGame();
        sceneAudioSetter?.PlayConfirmSound();
        Debug.Log("✅ 開始遊戲！");
        startBtn.interactable = false;
        startConfirmationPopup.SetActive(false); // ✅ 關閉視窗
    }

    private void OnNoButtonClicked()
    {
        sceneAudioSetter?.PlayConfirmSound(); // 如果你有取消音效
        startConfirmationPopup.SetActive(false); // ✅ 關閉視窗
         EventSystem.current.SetSelectedGameObject(startBtn.gameObject);
    }
    public void SetStartBtnVisible(bool isVisible)
    {
        startBtn.gameObject.SetActive(isVisible);
    }
    public void SwitchMenuType(MenuType menuType)
    {
        switch (menuType)
        {
            case MenuType.Menu:
                menuPanel.SetActive(true);
                roomPanel.SetActive(false);
                break;
            case MenuType.Room:
                menuPanel.SetActive(false);
                roomPanel.SetActive(true);
                EventSystem.current.SetSelectedGameObject(startBtn.gameObject);
                break;
            default:
                break;
        }
    }
    [SerializeField]
    private PlayerListCell playerListCell;

    private List<PlayerListCell> existingCells = new List<PlayerListCell>();

    public void UpdatePlayerList(List<GameManager.PlayerDisplayInfo> playerInfos)
    {
        foreach (var cell in existingCells)
        {
            Destroy(cell.gameObject);
        }
        existingCells.Clear();

        foreach (var info in playerInfos)
        {
            var cell = Instantiate(playerListCell, playerListContent.transform);
            cell.gameObject.SetActive(true);
            cell.SetPlayerName(info.Name);
            cell.SetPlayerAvatar(info.CharacterIndex);

            existingCells.Add(cell);
        }
    }
    public void ChooseCharacter(int characterIndex)
    {
        var gameManager = GameManager.Instance;

        if (characterIndex >= 0)
        {
            sceneAudioSetter?.PlayHoverSound();
            gameManager.SelectedCharacterIndex = characterIndex;

            var myPlayerRef = gameManager.networkRunner.LocalPlayer;
            if (gameManager.PlayerList.TryGetValue(myPlayerRef, out var myPlayerNetworkData))
            {
                myPlayerNetworkData.SelectedCharacterIndex = characterIndex;
            }

            for (int i = 0; i < characters3d.Length; i++)
            {
                characters3d[i].SetActive(false);
                charactersNameImage[i].SetActive(false);
            }

            characters3d[characterIndex].SetActive(true);
            charactersNameImage[characterIndex].SetActive(true);

            //characterName.text = charactersName[characterIndex];
            playerName = charactersName[characterIndex];


        }


    }


}
