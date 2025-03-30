using UnityEngine;
using Fusion;
using static UnityEngine.InputSystem.InputAction;

public class InputHandler : NetworkBehaviour
{
    private Vector2 moveInput; // 儲存移動輸入
    private bool damageTriggered; // 儲存按下空白鍵的結果
    public bool respawnTrigger;  // 用於偵測 K 鍵


    private void Update()
    {
        // 確保只有本地玩家才進行偵測
        if (!Object.HasInputAuthority)
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
            direction = new Vector3(moveInput.x, 0, moveInput.y),
            damageTrigger = damageTriggered,
            respawnTrigger = respawnTrigger
        };

        input.Set(data);
        // 傳送完畢後重置狀態，確保只會傳送一次按鍵事件
        damageTriggered = false;
        respawnTrigger = false;
    }
}

