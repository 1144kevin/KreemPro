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
    public GameObject[] CharacterPrefabs;// 新增角色預製體陣列
    public string RoomName { get; set; }

    public Dictionary<PlayerRef, PlayerNetworkData> PlayerList => playerList;
    private Dictionary<PlayerRef, PlayerNetworkData> playerList = new Dictionary<PlayerRef, PlayerNetworkData>();

    public struct PlayerDisplayInfo
    {
        public string Name;
        public int CharacterIndex;
    }

    private void Awake()//單例模式
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
    internal void SetPlayerNetworkData(PlayerRef player, PlayerNetworkData playerNetworkData)//設定玩家的網路物件
    {
        playerList.Add(player, playerNetworkData);
        playerNetworkData.transform.SetParent(transform);
    }

    private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)//玩家加入事件
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
            Debug.LogError("Failed to join lobby");
        }
        

    }


    public async Task CreateRoom()
    {
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        
        var result = await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = GameMode.AutoHostOrClient,
            SessionName = RoomName,
            PlayerCount = 4,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>(),
            //ObjectPool = gameObject.AddComponent<FusionObjectPoolRoot>()
        });


        if (result.Ok)
        {
            var menuManager = FindObjectOfType<MenuManager>();

            menuManager.SwitchMenuType(MenuManager.MenuType.Room);
            bool isHost = networkRunner.IsServer;
            menuManager.SetStartBtnVisible(isHost);
            if (!isHost)
            {
                var myRef = networkRunner.LocalPlayer;
                var conn = networkRunner.GetPlayerConnectionType(myRef);
                Debug.Log(conn == ConnectionType.Relayed
                          ? "目前使用 Photon 中繼 (Relay)"
                          : "已建立 Client ⇄ Host 直連 (P2P)");
            }
            else
            {
                Debug.Log("Host 直連 (P2P)");
            }
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
            //ObjectPool = gameObject.AddComponent<FusionObjectPoolRoot>()
        });

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
