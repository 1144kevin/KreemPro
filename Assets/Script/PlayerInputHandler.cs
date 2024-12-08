using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public class PlayerInputHandler : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerController playerController;

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
        var index = playerInput.playerIndex;
        playerController = players.FirstOrDefault(m => m.GetPlayerIndex() == index);
    }

    public void OnMove(CallbackContext context)
    {
        if (playerController != null)
        {
            var input = context.ReadValue<Vector2>();
            playerController.SetInputVector(context.ReadValue<Vector2>());
        }
    }

    public void OnAttack(CallbackContext context)
    {
        if (playerController != null)
        {
            // 在輸入開始時觸發攻擊，按鍵釋放時設為 false
            if (context.phase == InputActionPhase.Started)
            {
                playerController.SetAttackInput(true);
            }
            else if (context.phase == InputActionPhase.Canceled)
            {
                playerController.SetAttackInput(false);
            }
        }
    }
}
