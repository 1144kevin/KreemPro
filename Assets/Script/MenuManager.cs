using System;
using System.Collections.Generic;
using Fusion;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private GameObject menuPanel;

    [SerializeField]
    private GameObject roomPanel;

    [SerializeField]
    private InputField playerNameInputField;

    [SerializeField]
    private InputField roomNameInputField;

    [SerializeField]
    private Button createBtn;

    [SerializeField]
    private Button joinBtn;
    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private GameObject playerListContent;

    [SerializeField]
    private TMP_Text characterName;

    [SerializeField]
    private PlayerListCell playerListCell;

    private List<PlayerListCell> existingCells = new List<PlayerListCell>();

    public enum MenuType
    {
        Menu,
        Room
    }
    private void Start()
    {
        createBtn.onClick.AddListener(OnCreateBtnClicked);
        joinBtn.onClick.AddListener(OnJoinBtnClicked);
        startBtn.onClick.AddListener(OnStartBtnClicked);
    }
    private void OnDestroy()
    {
        createBtn.onClick.RemoveAllListeners();
        joinBtn.onClick.RemoveAllListeners();
        startBtn.onClick.RemoveAllListeners();
    }

    private async void OnCreateBtnClicked()
    {
        GameManager.Instance.PlayerName = playerNameInputField.text;
        GameManager.Instance.RoomName = roomNameInputField.text;
        await GameManager.Instance.CreateRoom();

    }
    private async void OnJoinBtnClicked()
    {
        GameManager.Instance.PlayerName = playerNameInputField.text;
        GameManager.Instance.RoomName = roomNameInputField.text;
        await GameManager.Instance.JoinRoom();
    }
    private void OnStartBtnClicked()
    {
        GameManager.Instance.StartGame();
        Debug.Log("Start");
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
                break;
            default:
                break;
        }
    }

    public void UpdatePlayerList(List<string> playerNames)
    {
        foreach (var cell in existingCells)
        {
            Destroy(cell.gameObject);
        }
        existingCells.Clear();

        foreach (var playerName in playerNames)
        {
            var cell = Instantiate(playerListCell, playerListContent.transform);
            cell.gameObject.SetActive(true);
            cell.SetPlayerName(playerName);
            existingCells.Add(cell);
        }
    }
    public void ChooseCharacter(int characterIndex)
    {
        var gameManager = GameManager.Instance;

        if (characterIndex >= 0)
        {
            gameManager.SelectedCharacterIndex = characterIndex;
            GameObject selectedPrefab = gameManager.CharacterPrefabs[characterIndex];

            characterName.text = selectedPrefab.name.Substring(0, selectedPrefab.name.Length - 6);
        }
    }

}
