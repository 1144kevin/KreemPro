using UnityEngine;
using Fusion;
using static UnityEngine.InputSystem.InputAction;


public class InputHandler : NetworkBehaviour
{
    private Vector2 moveInput;
    private bool damageTriggered;
    public bool respawnTrigger;

    public bool inputEnabled = true; // ✅ 改為非靜態（每位玩家自己有自己的值）

    private bool attackInput;
    private bool boosterTriggered;

    public void DisableInput()
    {
        inputEnabled = false;
    }

    public override void Spawned()
    {
        if (Runner.LocalPlayer != Object.InputAuthority) return;
        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.AddListener(OnInput);
    }

    public override void Despawned(NetworkRunner runner, bool hasState)
    {
        if (Runner.LocalPlayer != Object.InputAuthority) return;
        var events = Runner.GetComponent<NetworkEvents>();
        events.OnInput.RemoveListener(OnInput);
    }
    public void OnMove(CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        var cam = FindObjectOfType<CameraFollower>();
        if (cam != null)
        {
            cam.EnableCameraClamp();  // 死亡重生時先關掉 clamp
        }
    }

    public void OnDamage(CallbackContext context)
    {
        if (context.performed)
        {
            damageTriggered = true;
        }
    }

    // Respawn Action 的回呼
    public void OnRespawn(CallbackContext context)
    {
        if (context.performed)
        {
            respawnTrigger = true;
        }
    }

    public void OnAttack(CallbackContext context)
    {
        if (context.performed)
        {
            attackInput = true;
        }
    }
   public void OnBooster(CallbackContext context)
{
    if (context.performed)
    {
        boosterTriggered = true;
        Debug.Log("✅ Booster input received (F or X)");
    }
}
public void OnInput(NetworkRunner runner, NetworkInput input)
{
    if (!inputEnabled)
    {
        input.Set(new NetworkInputData());
        return;
    }

    var buttons = new NetworkButtons();

        var data = new NetworkInputData
        {
            direction = new Vector3(moveInput.x, 0, moveInput.y),
            damageTrigger = damageTriggered,
            respawnTrigger = respawnTrigger,
             boostTrigger = boosterTriggered, // 👈 加入這行
            buttons = buttons,
        };

        if (attackInput)
        {
            data.buttons.Set(InputButton.ATTACK, attackInput);
            attackInput = false;
        }

    input.Set(data);

    // Reset one-time triggers
    damageTriggered = false;
    respawnTrigger = false;
    boosterTriggered = false; // ✅ 別忘了重設這個
}


}

