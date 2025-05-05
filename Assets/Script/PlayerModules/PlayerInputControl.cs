using Fusion;
using UnityEngine;
using System;

public class PlayerInputControl : MonoBehaviour
{
    public event Action<NetworkInputData, NetworkButtons> OnInputReceived;


    private NetworkButtons previousButtons;

    public void HandleInput(NetworkRunner runner, PlayerRef playerRef)
    {
        NetworkInputData? input = runner.GetInputForPlayer<NetworkInputData>(playerRef);

        if (input == null)
            return;

        var pressed = input.Value.buttons.GetPressed(previousButtons);
        previousButtons = input.Value.buttons;

        OnInputReceived?.Invoke(input.Value, pressed);

    }

}
