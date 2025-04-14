using Fusion;
using UnityEngine;
using TMPro;

public class Player : NetworkBehaviour
{
  [SerializeField] private NetworkCharacterController CharacterController;
  [SerializeField] private int MaxHealth = 100;
  [SerializeField] private HealthBar HealthBar;
  [SerializeField] private AttackHandler AttackHandler;
  [SerializeField] private AnimationHandler AnimationHandler;
  [SerializeField] private float Speed = 500f;
  [Networked] private int Health { get; set; }
  [Networked] private bool isDead { get; set; } = false;
  [Networked] private NetworkButtons previousButton { get; set; }
  [SerializeField] private Camera playerCamera;
  [SerializeField] private GameObject respawnCanvas;
  [Networked] private Vector3 LastDeathPosition { get; set; }
  [Networked] public int kreemCollect { get; set; } = 0;
  private PlayerRespawn playerRespawn;
  private bool lastMoving = false;

  [SerializeField] private TMP_Text kreemText;



  public override void Spawned()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
    playerRespawn = GetComponent<PlayerRespawn>();
    if (playerRespawn == null)
    {
      Debug.LogError("PlayerRespawn component not found on player!");
    }
    
    if (Object.HasInputAuthority)
    {
      playerCamera.gameObject.SetActive(true);
      playerCamera.enabled = true;

    }
    else
    {
      playerCamera.gameObject.SetActive(false);
      playerCamera.enabled = false;
    }
    CreateKreemUI();

    // 初始血量
    Health = MaxHealth;

    // 只有 State Authority 呼叫 RpcUpdateHealth
    if (Object.HasStateAuthority)
    {
      RpcUpdateHealth(Health);

    }
    else
    {
      // 非 State Authority 的本地端，直接更新 HealthBar
      if (HealthBar != null)
      {
        HealthBar.SetHealth(Health);
      }
    }

    // 本地 UI 先隱藏
    if (respawnCanvas != null)
      respawnCanvas.SetActive(false);
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

  [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
  public void RpcUpdateAnimationState(Vector3 input)
  {
    AnimationHandler.PlayerAnimation(input);
  }

  public override void FixedUpdateNetwork()
  {
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
      var buttonPressed = data.button.GetPressed(previousButton);
      previousButton = data.button;

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

      if (data.damageTrigger && Health > 0)
      {
        TakeDamage(10);
      }

      if (Object.HasInputAuthority && Health <= 0 && data.respawnTrigger)
      {
        playerRespawn.RpcRequestRespawn();
      }

    }

    // 本地玩家根據 Health 控制死亡 UI 的顯示
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


}
