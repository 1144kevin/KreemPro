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
  [SerializeField] private AttackHandler AttackHandler;
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
  [SerializeField] private SceneAudioSetter sceneAudioSetter;
  [SerializeField] private int characterSoundIndex = 0; // 攻擊音效用的角色 ID
  private bool attackLocked = false;        // 攻擊鎖定旗標
  [SerializeField] private float attackCooldown = 0.5f;      // 根據角色 name 指定的延遲時間

  private void Awake()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
    playerRespawn = GetComponent<PlayerRespawn>();
    if (playerRespawn == null)
    {
      Debug.LogError("PlayerRespawn component not found!");
    }
  }

  public override void Spawned()
  {
    if (Object.HasInputAuthority)
    {
      // 只有本地玩家的相機會啟用
      StartCoroutine(EnableStartUI());
      sceneAudioSetter?.PlayRingSound();
      playerCamera.enabled = true;
      playerCamera.gameObject.SetActive(true);
    }
    else
    {
      playerCamera.enabled = false;
      playerCamera.gameObject.SetActive(false); ;
    }

    CreateKreemUI();
    Health = MaxHealth;

    if (Object.HasStateAuthority)
    {
      RpcUpdateHealth(Health);
    }
    else if (HealthBar != null)
    {
      HealthBar.SetHealth(Health);
    }

    if (respawnCanvas != null)
      respawnCanvas.SetActive(false);
  }

  private void UnlockAttack()
  {
    attackLocked = false;
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

  public void DisableCameraClampClient()
  {
    var cam = FindObjectOfType<CameraFollower>();
    if (cam != null)
      cam.DisableCameraClamp();
  }

  public void Heal(int amount)
  {
    if (!Object.HasStateAuthority) return;           // 確保只有 StateAuthority（Host）執行
    Health = Mathf.Clamp(Health + amount, 0, MaxHealth);
    RpcUpdateHealth(Health);                        // 同步給所有客戶端
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
    if (Object.HasStateAuthority && !isDead && transform.position.y < -400f)
    {
      isDead = true;
      playerRespawn.RpcSetPlayerVisibility(false);
    }

    if (Object.HasStateAuthority && Health <= 0 && !isDead)
    {

      isDead = true;
      LastDeathPosition = transform.position;
      playerRespawn.RpcSetPlayerVisibility(false);
      sceneAudioSetter?.PlayDieSound();
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
      if (buttonPressed.IsSet((int)InputButton.ATTACK) && !isDead && !attackLocked)
      {
        attackLocked = true;           // 立刻鎖住
        Invoke(nameof(UnlockAttack), attackCooldown);  // 延遲解鎖

        bool isRunning = data.direction.magnitude > 0.1f;

        if (Object.HasStateAuthority)
          RpcPlayAttackAnimation(isRunning);

        if (Object.HasInputAuthority)
          AttackHandler.Attack();
        
      }
      if (!isDead)
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

      // if (data.damageTrigger && !isDead)
      // {
      //   TakeDamage(10);
      // }

      // if (Object.HasInputAuthority && isDead && data.respawnTrigger)
      // {
      //   playerRespawn.RpcRequestRespawn();
      //   DisableCameraClampClient();
      // }
    }

    if (Object.HasInputAuthority)
    {
      if (isDead)
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



  private IEnumerator EnableStartUI()
  {
    {
      if (Object.HasInputAuthority)
      {
        var ui = GameObject.Find("StartGameUI");
        if (ui != null)
        {
          ui.SetActive(true);
          ui.transform.localScale = Vector3.zero; // 先縮到 0 大小

          // 彈出效果（0.5秒放大到正常大小）
          LeanTween.scale(ui, Vector3.one, 0.3f).setEaseOutBack();

          yield return new WaitForSeconds(startGameTime - 0.2f); // 留時間給縮小動畫

          // 縮小效果（0.5秒縮小到 0）
          LeanTween.scale(ui, Vector3.zero, 0.2f).setEaseInBack();

          yield return new WaitForSeconds(0.2f); // 等縮小完
          ui.SetActive(false);
        }
      }
    }
  }
}
