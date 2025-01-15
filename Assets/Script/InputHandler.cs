using UnityEngine;
using Fusion;


public class InputHandler : NetworkBehaviour
{
    
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
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
            data.direction += Vector3.forward;

        if (Input.GetKey(KeyCode.S))
            data.direction += Vector3.back;

        if (Input.GetKey(KeyCode.A))
            data.direction += Vector3.left;

        if (Input.GetKey(KeyCode.D))
            data.direction += Vector3.right;

        data.button.Set(InputButton.ATTACK, Input.GetMouseButton(0));

        input.Set(data);
    }
}

