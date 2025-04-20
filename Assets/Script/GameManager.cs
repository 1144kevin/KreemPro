using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField]
    private NetworkRunner networkRunner;
    [SerializeField]
    private NetworkEvents networkEvents;
    public string PlayerName { get; set; }
    public int SelectedCharacterIndex { get; set; }

    // 新增角色預製體陣列
    public GameObject[] CharacterPrefabs;
    public string RoomName { get; set; }

    [SerializeField]
    private PlayerNetworkData playerNetworkDataPrefab;

    public Dictionary<PlayerRef, PlayerNetworkData> PlayerList => playerList;
    private Dictionary<PlayerRef, PlayerNetworkData> playerList = new Dictionary<PlayerRef, PlayerNetworkData>();

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            networkEvents.PlayerJoined.AddListener(OnPlayerJoined);
            networkEvents.PlayerLeft.AddListener(OnPlayerLeft);
            DontDestroyOnLoad(gameObject);
        // ✅ 監聽場景切換事件
        }
        else
        {
            Destroy(gameObject);
        }
    }
    internal void SetPlayerNetworkData(PlayerRef player, PlayerNetworkData playerNetworkData)
    {
        playerList.Add(player, playerNetworkData);
        playerNetworkData.transform.SetParent(transform);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer)
        {
            Debug.LogWarning("Only the server can handle player joining!");
            return;
        }
        runner.Spawn(playerNetworkDataPrefab, transform.position, Quaternion.identity, player);

    }
    private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Host 執行移除玩家資料
        if (playerList.TryGetValue(player, out var playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);
            playerList.Remove(player);
        }

}
// public  void OnShutdown()
// {
//     Debug.LogWarning("❗ Fusion Shutdown 被呼叫（可能是 Host 離開）");

//     if (!networkRunner.IsServer)
//     {
//         Debug.Log("📌 Client 偵測 Host 離線，自動跳轉 Entry Scene");
//         SceneManager.LoadScene("Entry");
//     }
// }


    private async void Start()
    {
        var result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Debug.LogError("shit");
        }
    }

    public async Task CreateRoom()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = RoomName,
            PlayerCount = 4,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });


        if (result.Ok)
        {
            var menuManager = FindObjectOfType<MenuManager>();

            menuManager.SwitchMenuType(MenuManager.MenuType.Room);
            menuManager.SetStartBtnVisible(true);
        }
        else
        {
            Debug.LogError("Failed to create room.");
        }
    }

    public async Task JoinRoom()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = RoomName,
            PlayerCount = 4,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
        });

        if (result.Ok)
        {
            var menuManager = FindObjectOfType<MenuManager>();

            menuManager.SwitchMenuType(MenuManager.MenuType.Room);
            menuManager.SetStartBtnVisible(false);
        }
        else
        {
            Debug.LogError("Failed to join room.");
        }
    }

    public void UpdatePlayerList()
    {
        var playerNames = new List<string>();
        foreach (var playerNetworkData in playerList.Values)
        {
            playerNames.Add(playerNetworkData.PlayerName);
        }

        var menuManager = FindObjectOfType<MenuManager>();
        menuManager.UpdatePlayerList(playerNames);
    }
public void StartGame()
{
    networkRunner.LoadScene("Entry");
    Debug.Log("📦 Host 已執行 LoadScene('Entry')");
}




}
