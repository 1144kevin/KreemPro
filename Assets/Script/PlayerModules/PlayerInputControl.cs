using Fusion;
using UnityEngine;
using System;

public class PlayerInputControl :  NetworkBehaviour
{
    public event Action<NetworkInputData, NetworkButtons> OnInputReceived;
    public NetworkInputData LatestInput { get; private set; }

    private NetworkButtons previousButtons;


    public void HandleInput(NetworkRunner runner, PlayerRef playerRef)
    {
        NetworkInputData? input = runner.GetInputForPlayer<NetworkInputData>(playerRef);

        if (input == null)
            return;

        var pressed = input.Value.buttons.GetPressed(previousButtons);
        previousButtons = input.Value.buttons;

        LatestInput = input.Value;

        OnInputReceived?.Invoke(input.Value, pressed);
    }

}
