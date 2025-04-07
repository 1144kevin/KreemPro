using UnityEngine;
using Fusion;
using static UnityEngine.InputSystem.InputAction;

public class InputHandler : NetworkBehaviour
{
    private Vector2 moveInput;
    private bool damageTriggered;
    public bool respawnTrigger;

    public  bool inputEnabled = true; // ✅ 改為非靜態（每位玩家自己有自己的值）

    private void Update()
    {
        if (!Object.HasInputAuthority || !inputEnabled)
            return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            damageTriggered = true;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            respawnTrigger = true;
        }
    }
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
    }
public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        if (!inputEnabled)
        {
            input.Set(new NetworkInputData());
            return;
        }

    var data = new NetworkInputData
    {
        direction = new Vector3(moveInput.x, 0, moveInput.y),
        damageTrigger = damageTriggered,
        respawnTrigger = respawnTrigger
    };

    input.Set(data);
    damageTriggered = false;
    respawnTrigger = false;
}

}

