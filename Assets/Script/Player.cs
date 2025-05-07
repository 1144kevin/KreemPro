using Fusion;
using UnityEngine;
public class Player : NetworkBehaviour
{
  // PlayerModule
  private PlayerInputControl inputHandler;
  private NewPlayerMovement playerMovement;
  private PlayerCombat playerCombat;
  private PlayerRespawn playerRespawn;
  private PlayerUIManager playerUI;
  public PlayerHealth playerHealth { get; private set; }
  public PlayerAudio playerAudio { get; private set; }
  public bool HasGameEnded { get; private set; } = false;

  [Header("Newtwork Objects Status")]
  [Networked] public int kreemCollect { get; set; } = 0;

  [Header("Game Objects")]
  [SerializeField] private NetworkCharacterController CharacterController;
  [SerializeField] private HealthBar HealthBar;
  [SerializeField] private Camera playerCamera;
  [SerializeField] private AttackHandler AttackHandler;
  [SerializeField] private AnimationHandler AnimationHandler;
  public NetworkPrefabRef kreemPrefab;

  [Header("Attack Effect")]
  [SerializeField] public ParticleSystem getHitEffect;
  [SerializeField] private ParticleSystem sharedHitEffect;

  [Header("Audio")]
  [SerializeField] private SceneAudioSetter sceneAudioSetter;
  [SerializeField] public int characterSoundIndex = 0; // 攻擊音效用的角色 ID

  [Header("Attack Direction UI")]
  [SerializeField] public Transform CharacterTrans;
  [SerializeField] private Transform attackDirectionUI; // 指向 UI 根物件
  [SerializeField] private float arrowDistance = 4f; // 前方距離
  [SerializeField] private float arrowHeight = 1.5f;  // 高度
  [SerializeField] private Vector3 arrowOffset = Vector3.zero; // 額外位置偏移

  public GameObject boosterUIPrefab; // 拖進一個預設 UI Prefab
  private GameObject boosterUIInstance;


  private void Update()
  {
    if (!Object.HasInputAuthority || attackDirectionUI == null || AttackHandler == null) return;

    Transform charTrans = AttackHandler.GetCharacterTrans();
    if (charTrans == null) return;

    Vector3 forward = charTrans.forward;
    Vector3 offset = forward * arrowDistance + Vector3.up * arrowHeight;

    // 加入世界空間偏移量（旋轉下的偏移）
    Vector3 adjustedOffset = charTrans.rotation * arrowOffset;

    attackDirectionUI.position = charTrans.position + offset + adjustedOffset;
  }

  private void Awake()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
    playerRespawn = GetComponent<PlayerRespawn>();
    inputHandler = GetComponent<PlayerInputControl>();
    playerMovement = GetComponent<NewPlayerMovement>();
    playerCombat = GetComponent<PlayerCombat>();
    playerHealth = GetComponent<PlayerHealth>();
    playerUI = GetComponent<PlayerUIManager>();
    playerAudio = GetComponent<PlayerAudio>();

    if (playerRespawn == null)
      Debug.LogError("PlayerRespawn component not found!");
    if (inputHandler == null)
      Debug.LogError("PlayerInputHandler not found on Player!");

    CharacterController = GetComponent<NetworkCharacterController>();
    playerRespawn = GetComponent<PlayerRespawn>();
    if (playerRespawn == null)
    {
      Debug.LogError("PlayerRespawn component not found!");
    }
  }

  public override void Spawned()
  {
    playerUI?.InitKreemText();

    if (Object.HasInputAuthority)
    {
      playerUI?.ShowStartGameUI();
    }
    playerHealth?.Init();
    if (inputHandler != null)
    {
      inputHandler.OnInputReceived += OnInputReceived;
    }
    //attackDirection權限
    if (Object.HasInputAuthority)
    {
      if (attackDirectionUI != null)
        attackDirectionUI.gameObject.SetActive(true);
    }
    else
    {
      if (attackDirectionUI != null)
        attackDirectionUI.gameObject.SetActive(false);
    }

    if (Object.HasInputAuthority)
    {
      // 只有本地玩家的相機會啟用
      playerUI?.ShowStartGameUI();
      playerCamera.enabled = true;
      playerCamera.gameObject.SetActive(true);
    }
    else
    {
      playerCamera.enabled = false;
      playerCamera.gameObject.SetActive(false); ;
    }

    if (Object.HasInputAuthority) // 只為自己的玩家產生 UI
    {
        boosterUIInstance = Instantiate(boosterUIPrefab);
        boosterUIInstance.GetComponent<BoosterUI>().booster = GetComponent<Booster>();
    }

    playerUI?.InitKreemText();
    playerHealth?.Revive();
  }


  private void OnInputReceived(NetworkInputData input, NetworkButtons pressed)
  {
    // playerMovement?.HandleMove(input, Runner);
    playerCombat?.HandleAttack(input, pressed, Runner);
  }

  // 透過 RPC 同步更新所有客戶端的 HealthBar
  [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
  public void RpcSetGameEnded()
  {
    HasGameEnded = true;

    playerUI?.RefreshRespawnUI();

    Debug.Log($"[RPC] 玩家 {Object.InputAuthority} 遊戲結束，已刷新 UI");
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcUpdateAnimationState(Vector3 input)
  {
    AnimationHandler.PlayerAnimation(input);
  }

  public void DisableCameraClampClient()
  {
    var cam = FindObjectOfType<CameraFollower>();
    if (cam != null)
      cam.DisableCameraClamp();
  }


  public override void FixedUpdateNetwork()
  {

    if (Object.HasInputAuthority)
      inputHandler?.HandleInput(Runner, Object.InputAuthority);

    if (Object.HasInputAuthority && !HasGameEnded)
      playerUI?.RefreshRespawnUI();

    // ✅ 統一處理移動
    var input = Runner.GetInputForPlayer<NetworkInputData>(Object.InputAuthority);
    if (input != null)
    {
      Vector3 direction = input.Value.direction.normalized;
      playerMovement?.HandleMovement(direction, Runner);
      playerCombat?.TickCombat(input.Value);
      playerHealth?.TickFallDeath(Runner); // ✅ 檢查邊界死亡
    }
  }

  public void TakeDamage(int damage)
  {
    playerHealth?.TakeDamage(damage);
  }

  // 給 Server 呼叫的加分邏輯
  public void ServerAddKreem()
  {
    if (!Object.HasStateAuthority) return;

    kreemCollect++;
    Debug.Log($"[Server] 玩家 {Object.InputAuthority} 撿到 Kreem：{kreemCollect}");

    if (playerUI == null)
    {
      Debug.LogWarning($"[{Object.InputAuthority}] 找不到 PlayerUIManager 在物件 {name}");
    }
    else
    {
      // playerUI 會自動在 LateUpdate() 顯示最新分數，無需再操作 kreemText
      Debug.Log("Kreem UI 更新由 PlayerUIManager 處理");
    }
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]

  //音效RPC
  public void RpcPlayKreemSound()
  {
    playerAudio?.PlayKreemSound(); // ✅ 這樣才由 Client 播音效
  }
}




