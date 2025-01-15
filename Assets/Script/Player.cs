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
  // [Networked(OnChanged = nameof(HealthChanged))]
  //  private int Health { get; set; }  
  [Networked] private int Health { get; set; }
  private int lastHealth; // 用於檢測健康值是否變化
  [Networked] private NetworkButtons previousButton { get; set; }

  // [SerializeField] private MeshRenderer[] Visuals;
  // // [SerializeField] private Camera Camera;
  // [Networked] private Angle Pitch { get; set; }
  // [Networked] private Angle Yaw { get; set; }
  private void Awake()
  {
    CharacterController = GetComponent<NetworkCharacterController>();
  }
  public override void Spawned()
  {
    // if (Object.HasInputAuthority)
    // {
    //   Camera.enabled = true;

    //   foreach (var visual in Visuals)
    //   {
    //     visual.enabled = false;
    //   }
    // }
    // else
    // {
    //   Camera.enabled = false;
    // }

    Health = MaxHealth;
    lastHealth = Health; // 初始化健康值記錄
    UpdateHealth(); // 初始更新血條

  }

  public override void FixedUpdateNetwork()
  {
    if (GetInput(out NetworkInputData data))
    {
      var buttonPressed = data.button.GetPressed(previousButton);
      previousButton = data.button;

      data.direction.Normalize();
      CharacterController.Move(Speed * data.direction * Runner.DeltaTime);

      AnimationHandler.PlayerAnimation(data.direction);

      if (buttonPressed.IsSet(InputButton.ATTACK))
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