using Fusion;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon.StructWrapping; // ← 加這個

public class Player : NetworkBehaviour
{
  [SerializeField] private NetworkCharacterController CharacterController;
  [SerializeField] private int MaxHealth = 100;
  [SerializeField] private HealthBar HealthBar;
  [SerializeField] private AttackHandler2 AttackHandler;
  [SerializeField] private AnimationHandler AnimationHandler;
  [SerializeField] private float Speed = 500f;
  [Networked] private int Health { get; set; }
  [Networked] private bool isDead { get; set; } = false;
  [Networked] private NetworkButtons previousButton { get; set; }
  [SerializeField] private Camera playerCamera;
  [SerializeField] private GameObject respawnCanvas;
  [Networked] private Vector3 LastDeathPosition { get; set; }
  private bool hasGameEnded = false;
  [Networked] public int kreemCollect { get; set; } = 0;
  private PlayerRespawn playerRespawn;
  private bool lastMoving = false;
  [SerializeField] private float startGameTime = 2.0f;
  [SerializeField] private TMP_Text kreemText;



  public override void Spawned()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
    playerRespawn = GetComponent<PlayerRespawn>();
    AttackHandler = GetComponentInChildren<AttackHandler2>(true); 
    
    if (AttackHandler != null)
{
    Debug.Log($"[Player.Spawned] 🎯 AttackHandler 綁定成功：{AttackHandler.name} | ID: {AttackHandler.GetInstanceID()}", AttackHandler);
}
else
{
    Debug.LogError("[Player.Spawned] ❌ 無法綁定 AttackHandler2");
}

    if (playerRespawn == null)
      Debug.LogError("PlayerRespawn component not found!");

    CreateKreemUI();
    Health = MaxHealth;

    if (Object.HasStateAuthority)
      RpcUpdateHealth(Health);
    else if (HealthBar != null)
      HealthBar.SetHealth(Health);

    if (respawnCanvas != null)
      respawnCanvas.SetActive(false);

    // ✅ 只對自己的角色啟動相機
    if (Object.HasInputAuthority)
      StartCoroutine(EnableCameraAfterTransformReady());
    else
    {
      playerCamera.enabled = false;
      playerCamera.gameObject.SetActive(false);
    }
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcPlayAttackAnimation(bool isRunning)
  {
    AnimationHandler.TriggerAttack(isRunning);
  }


  // 透過 RPC 同步更新所有客戶端的 HealthBar
  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcUpdateHealth(int currentHealth)
  {
    if (HealthBar != null)
    {
      HealthBar.SetHealth(currentHealth);
    }
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
  public void RpcSetGameEnded()
  {
    hasGameEnded = true;  // 直接更新本地旗標，不使用 Networked localGameEnded
    if (respawnCanvas != null && respawnCanvas.activeSelf)
    {
      respawnCanvas.SetActive(false);
    }
    Debug.Log($"[RPC] 玩家 {Object.InputAuthority} 遊戲結束，已關閉 respawnCanvas");
  }

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcUpdateAnimationState(Vector3 input)
  {
    AnimationHandler.PlayerAnimation(input);
  }

  public override void FixedUpdateNetwork()
  {
    if (Object.HasInputAuthority)
    {
      // 如果已經收到遊戲結束的 RPC，直接關閉 canvas 並提前返回
      if (hasGameEnded)
      {
        if (respawnCanvas != null && respawnCanvas.activeSelf)
          respawnCanvas.SetActive(false);
        return;
      }
    }

    if (Object.HasStateAuthority && Health <= 0 && !isDead)
    {
      isDead = true;
      LastDeathPosition = transform.position;
      playerRespawn.RpcSetPlayerVisibility(false);
      if (playerRespawn.KreemPrefab != null)
      {
        Runner.Spawn(playerRespawn.KreemPrefab, LastDeathPosition, Quaternion.identity, default(PlayerRef));
      }
    }

    if (GetInput(out NetworkInputData data))
    {
      var buttonPressed = data.buttons.GetPressed(previousButton);
      previousButton = data.buttons;

      // 播放攻擊動畫（只針對本地玩家）
      if (Object.HasStateAuthority && buttonPressed.IsSet((int)InputButton.ATTACK) && Health > 0)
      {
        bool isRunning = data.direction.magnitude > 0.1f;
        RpcPlayAttackAnimation(isRunning);
      }
      if (Health > 0)
      {
        data.direction.Normalize();
        CharacterController.Move(Speed * data.direction * Runner.DeltaTime);

        bool currentMoving = data.direction.magnitude > 0.1f;

        if (Object.HasStateAuthority && currentMoving != lastMoving)
        {
          lastMoving = currentMoving;
          RpcUpdateAnimationState(currentMoving ? data.direction : Vector3.zero);
        }
        if (Object.HasInputAuthority)
        {
          AnimationHandler.PlayerAnimation(data.direction);
        }
      }

      if (data.buttons.IsSet(InputButton.ATTACK))
      {
          Debug.Log("🔘 ATTACK input 被偵測到了");

          if (AttackHandler != null && Object.HasInputAuthority)
          {
              Debug.Log("📤 客戶端送出攻擊請求");
              AttackHandler.RequestAttackRpc();
          }
      }


      if (data.damageTrigger && Health > 0)
      {
        TakeDamage(10);
      }

      if (Object.HasInputAuthority && Health <= 0 && data.respawnTrigger)
      {
        playerRespawn.RpcRequestRespawn();
      }
    }

    if (Object.HasInputAuthority)
    {
      if (Health <= 0)
      {
        if (respawnCanvas != null && !respawnCanvas.activeSelf)
          respawnCanvas.SetActive(true);
      }
      else
      {
        if (respawnCanvas != null && respawnCanvas.activeSelf)
          respawnCanvas.SetActive(false);
      }
    }
  }

  private void LateUpdate()
  {
    // 不管是哪一端都跑，顯示頭上數字
    if (kreemText != null)
    {
      kreemText.text = kreemCollect.ToString();
    }
  }

  public void TakeDamage(int damage)
  {
    if (!Object.HasStateAuthority) return;

    Health -= damage;
    Health = Mathf.Clamp(Health, 0, MaxHealth);
    Debug.Log("hit");

    RpcUpdateHealth(Health);
  }

  // 當重生完成後，重置 Health 與死亡狀態，並顯示角色
  public void SetHealthToMax()
  {
    if (!Object.HasStateAuthority) return;
    Health = MaxHealth;
    isDead = false;
    RpcUpdateHealth(Health);
    playerRespawn.RpcSetPlayerVisibility(true);
  }

  // 給 Server 呼叫的加分邏輯
  public void ServerAddKreem()
  {
    if (!Object.HasStateAuthority) return;

    kreemCollect++;
    Debug.Log($"[Server] 玩家 {Object.InputAuthority} 撿到 Kreem：{kreemCollect}");
  }

  private void CreateKreemUI()
  {
    var canvas = GetComponentInChildren<Canvas>(true);
    if (canvas != null)
    {
      var tmps = canvas.GetComponentsInChildren<TMP_Text>(true);
      foreach (var tmp in tmps)
      {
        if (tmp.name.Contains("Kreem"))
        {
          kreemText = tmp;
          Debug.Log($"[{Object.InputAuthority}] 成功綁定 TMP_Text：{kreemText.name} 在物件 {name}");
          break;
        }
      }

      if (kreemText == null)
        Debug.LogWarning($"[{Object.InputAuthority}] 找不到 KreemText 在物件 {name}");
    }
    else
    {
      Debug.LogWarning("找不到 Canvas");
    }
  }


  private IEnumerator EnableCameraAfterTransformReady()
  {
    // 等待 transform 初始化完成（避免為 Vector3.zero）
    while (transform.position.sqrMagnitude < 10f)
      yield return null;

    var follower = playerCamera.GetComponent<CameraFollower>();
    Vector3 offset = follower != null ? follower.offset : new Vector3(0, -800, 500);

    playerCamera.transform.position = transform.position - offset;
    playerCamera.gameObject.SetActive(true);
    playerCamera.enabled = true;

    if (follower != null)
      follower.SetTarget(transform);

    Debug.Log($"📸 相機啟動完成：{transform.position}");
    if (Object.HasInputAuthority)
    {
      var ui = GameObject.Find("StartGameUI");
      if (ui != null)
        ui.SetActive(true);
      yield return new WaitForSeconds(startGameTime); //  // ✅ 特定秒數後自動隱藏，可自訂秒數
      ui.SetActive(false);
    }

  }




}
