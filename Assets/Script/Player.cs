using Fusion;
using UnityEngine;

public class Player : NetworkBehaviour
{
  [SerializeField] private NetworkCharacterController CharacterController;
  [SerializeField] private int MaxHealth = 100;
  [SerializeField] private HealthBar HealthBar;
  [SerializeField] private AttackHandler AttackHandler;
  [SerializeField] private AnimationHandler AnimationHandler;
  [SerializeField] private float Speed = 500f;
  [SerializeField] private Camera playerCamera;

  [Networked] private int Health { get; set; }
  [Networked] private NetworkButtons previousButton { get; set; }

  private int lastHealth; // 用於檢測健康值是否變化

  private void Awake()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
    AttackHandler= GetComponent<AttackHandler>();
  }
  public override void Spawned()
  {
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

    Health = MaxHealth;
    lastHealth = Health; // 初始化健康值記錄
    UpdateHealth(); // 初始更新血條

  }

  public override void FixedUpdateNetwork()
  {
    if (GetInput(out NetworkInputData data))
    {
      previousButton = data.buttons;

      data.direction.Normalize();
      CharacterController.Move(Speed * data.direction * Runner.DeltaTime);

      AnimationHandler.PlayerAnimation(data.direction);
      
      if (data.buttons.IsSet(InputButton.ATTACK))
      {
        AttackHandler.Attack();
      }
    }
    // 健康值變化檢測
    if (Health != lastHealth)
    {
      UpdateHealth();
      lastHealth = Health; // 更新記錄的健康值
    }
    if (Health <= 0)
    {
      Dead();
    }
    if (Input.GetKey(KeyCode.Space))
    {
      TakeDamage(10);
    }
  }

  public void TakeDamage(int damage)
  {
    Health -= damage;
    Health = Mathf.Clamp(Health, 0, MaxHealth); // 確保健康值範圍
    Debug.Log("hit");
  }

  private void Dead()
  {
    Health = MaxHealth;
    CharacterController.transform.position = new Vector3(0, 0, 0);
  }

  // private static void HealthChanged(Changed<Player> changed)
  // {
  //   changed.Behaviour.UpdateHealth();
  // }

  private void UpdateHealth()
  {
    HealthBar.SetHealth(Health);
  }
}