using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public NetworkRunner networkRunner;

    [SerializeField]
    private NetworkEvents networkEvents;

    [SerializeField]
    private PlayerNetworkData playerNetworkDataPrefab;

    public static GameManager Instance { get; private set; }
    public string PlayerName { get; set; }
    public int SelectedCharacterIndex { get; set; }
    public GameObject[] CharacterPrefabs;
    public string RoomName { get; set; }

    public Dictionary<PlayerRef, PlayerNetworkData> PlayerList => playerList;
    private Dictionary<PlayerRef, PlayerNetworkData> playerList = new Dictionary<PlayerRef, PlayerNetworkData>();

    public struct PlayerDisplayInfo
    {
        public string Name;
        public int CharacterIndex;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            networkEvents.PlayerJoined.AddListener(OnPlayerJoined);
            networkEvents.PlayerLeft.AddListener(OnPlayerLeft);
            DontDestroyOnLoad(gameObject);
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
        if (playerList.TryGetValue(player, out var playerNetworkData))
        {
            runner.Despawn(playerNetworkData.Object);
            playerList.Remove(player);
        }
    }

    private async void Start()
    {
        var result = await networkRunner.JoinSessionLobby(SessionLobby.ClientServer);

        if (result.Ok)
        {
            SceneManager.LoadScene("Menu");
        }
        else
        {
            Debug.LogError("Failed to join lobby");
        }
    }

    public async Task CreateRoom()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        // ✅ 確保 networkRunner 和 SceneManager 掛在同一 GameObject
        var sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Host,
            SessionName = RoomName,
            PlayerCount = 4,
            Scene = scene,
            SceneManager = sceneManager
        });

        Debug.Log($"[CreateRoom] Runner Mode: {networkRunner.Mode}, LagComp: {networkRunner.LagCompensation}, SceneMgr: {networkRunner.SceneManager}");

        if (result.Ok)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            menuManager.SwitchMenuType(MenuManager.MenuType.Room);
            menuManager.SetStartBtnVisible(true);
            Debug.Log("這是 Host (直連)");
        }
        else
        {
            Debug.LogError("Failed to create room.");
        }
    }

    public async Task JoinRoom()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        var sceneManager = networkRunner.gameObject.AddComponent<NetworkSceneManagerDefault>();

        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.Client,
            SessionName = RoomName,
            PlayerCount = 4,
            Scene = scene,
            SceneManager = sceneManager
        });

        Debug.Log($"[JoinRoom] Runner Mode: {networkRunner.Mode}, LagComp: {networkRunner.LagCompensation}, SceneMgr: {networkRunner.SceneManager}");

        if (result.Ok)
        {
            var menuManager = FindObjectOfType<MenuManager>();
            menuManager.SwitchMenuType(MenuManager.MenuType.Room);
            menuManager.SetStartBtnVisible(false);

            var myRef = networkRunner.LocalPlayer;
            var conn = networkRunner.GetPlayerConnectionType(myRef);
            Debug.Log(conn == ConnectionType.Relayed
                      ? "目前使用 Photon 中繼 (Relay)"
                      : "已建立 Client ⇄ Host 直連 (P2P)");
        }
        else
        {
            Debug.LogError("Failed to join room.");
        }
    }

    public void UpdatePlayerList()
    {
        var playerInfos = new List<PlayerDisplayInfo>();

        foreach (var kvp in playerList)
        {
            var playerRef = kvp.Key;
            var playerData = kvp.Value;
            int characterIndex = playerData.SelectedCharacterIndex;
            int playerId = playerRef.PlayerId;

            string displayName = $"{playerData.PlayerName} (Player {playerId})";
            playerInfos.Add(new PlayerDisplayInfo
            {
                Name = displayName,
                CharacterIndex = characterIndex
            });
        }

        var menuManager = FindObjectOfType<MenuManager>();
        menuManager.UpdatePlayerList(playerInfos);
    }

    public void StartGame()
    {
        networkRunner.LoadScene("Entry");
        Debug.Log("📦 Host 已執行 LoadScene('Entry')");
    }
}