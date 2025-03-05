using UnityEngine;
using Fusion;
using static UnityEngine.InputSystem.InputAction;

public class InputHandler : NetworkBehaviour
{
    private Vector2 moveInput; // 儲存移動輸入
    private bool attackInput;
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
        Debug.Log("move");
        moveInput = context.ReadValue<Vector2>();
    }
    public void OnAttack(CallbackContext context)//沒有運作
    {
        Debug.Log("attack");
        if (context.performed)
        {
            attackInput = true;  
        }
    }
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData
        {
            direction = new Vector3(moveInput.x, 0, moveInput.y)
            
        };
        
        //data.buttons.Set(InputButton.ATTACK,attackInput);
        // if (attackInput)  
        // {
        //     data.buttons.IsSet((int)InputButton.ATTACK);
            
        // }
        data.buttons.Set(InputButton.ATTACK,Input.GetMouseButton(0));//接收left mouse的資訊，不是從player input取得

        input.Set(data);
    }
}

