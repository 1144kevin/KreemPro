using Fusion;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon.StructWrapping; // ← 加這個





public class Player : NetworkBehaviour
{

  //NEW
  private PlayerInputControl inputHandler;
  private NewPlayerMovement playerMovement;
  private PlayerCombat playerCombat;
  public PlayerHealth playerHealth { get; private set; }
  private PlayerUIManager playerUI;
  public bool HasGameEnded => hasGameEnded;
  public PlayerAudio playerAudio { get; private set; }



  [SerializeField] private NetworkCharacterController CharacterController;
  [SerializeField] private int MaxHealth = 100;
  [SerializeField] private HealthBar HealthBar;

  [SerializeField] private AttackHandler AttackHandler;
  [SerializeField] private AnimationHandler AnimationHandler;
  [SerializeField] public float Speed = 500f;
  [SerializeField] private float debugSpeedOverride = -1f;
  [Networked] private NetworkButtons previousButton { get; set; }
  [SerializeField] private Camera playerCamera;
  private bool hasGameEnded = false;
  [Networked] public int kreemCollect { get; set; } = 0;
  private PlayerRespawn playerRespawn;
  private bool lastMoving = false;
  private bool attackLocked = false;        // 攻擊鎖定旗標

  [Header("Attack Effect")]
  [SerializeField] public ParticleSystem getHitEffect;
  [SerializeField] private ParticleSystem sharedHitEffect;

  [Header("Audio")]
  [SerializeField] private SceneAudioSetter sceneAudioSetter;
  [SerializeField] private int characterSoundIndex = 0; // 攻擊音效用的角色 ID

  [Header("Attack Direction UI")]
  [SerializeField] public Transform CharacterTrans;
  [SerializeField] private Transform attackDirectionUI; // 指向 UI 根物件
  [SerializeField] private float arrowDistance = 4f; // 前方距離
  [SerializeField] private float arrowHeight = 1.5f;  // 高度
  [SerializeField] private Vector3 arrowOffset = Vector3.zero; // 額外位置偏移


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
    //NEW
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
    //

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

    playerUI?.InitKreemText();
    playerHealth?.Revive();
  }

  // ✅ Health UI handled by PlayerHealth}

  private void OnInputReceived(NetworkInputData input, NetworkButtons pressed)
  {
    playerMovement?.HandleMove(input, Runner);
    playerCombat?.HandleAttack(input, pressed, Runner);
  }

  // 透過 RPC 同步更新所有客戶端的 HealthBar
  [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
  public void RpcSetGameEnded()
  {
    hasGameEnded = true; // 本地旗標

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

  public void Heal(int amount)
  {
    playerHealth?.Heal(amount);
  }

  public override void FixedUpdateNetwork()
  {
    // ✅ Step① - 由 PlayerInputControl 統一處理輸入
    if (inputHandler != null && Object.HasInputAuthority)
    {
      inputHandler.HandleInput(Runner, Object.InputAuthority);
    }

    // ✅ Step① - canvas控制（可保留）
    if (Object.HasInputAuthority)
    {
      if (hasGameEnded)
        return;

      playerUI?.RefreshRespawnUI();
    }
    playerUI?.RefreshRespawnUI();
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
  private void PlayHitEffectLocal()
  {
    if (getHitEffect != null)
    {
      if (!getHitEffect.gameObject.activeSelf)
        getHitEffect.gameObject.SetActive(true);

      getHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      getHitEffect.Play();

      // 自動關閉特效物件（延遲一點）
      StartCoroutine(DisableAfterSeconds(getHitEffect.gameObject, 0.5f));
    }
  }
  private IEnumerator DisableAfterSeconds(GameObject go, float delay)
  {
    yield return new WaitForSeconds(delay);
    if (go != null)
      go.SetActive(false);
  }


  [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
  public void RpcPlayHitEffect()
  {
    PlayHitEffectLocal();
  }

  private void PlaySharedHitEffectLocal()
  {
    if (sharedHitEffect != null)
    {
      Debug.Log("✅ PlaySharedHitEffectLocal: trying to play");

      // 不停用 GameObject，只停用粒子本身
      sharedHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
      sharedHitEffect.Play();

      // 如果你真的需要手動清除尾巴殘影，可加這行延遲清尾
      StartCoroutine(ClearSharedHitEffect(0.5f));
    }
  }

  private IEnumerator ClearSharedHitEffect(float delay)
  {
    yield return new WaitForSeconds(delay);
    if (sharedHitEffect != null)
    {
      sharedHitEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcPlaySharedHitEffect()
  {
    PlaySharedHitEffectLocal();
  }
}




