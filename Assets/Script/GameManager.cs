using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            //ObjectPool = gameObject.AddComponent<FusionObjectPoolRoot>()
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
            //ObjectPool = gameObject.AddComponent<FusionObjectPoolRoot>()
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
        networkRunner.LoadScene(SceneRef.FromIndex(2));//場景管理器待修
    }
}
