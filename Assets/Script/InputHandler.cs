using UnityEngine;
using Fusion;
using static UnityEngine.InputSystem.InputAction;

public class InputHandler : NetworkBehaviour
{
    private Vector2 moveInput; // 儲存移動輸入

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
        var data = new NetworkInputData
        {
            direction = new Vector3(moveInput.x, 0, moveInput.y)
        };

        input.Set(data);
    }
}

