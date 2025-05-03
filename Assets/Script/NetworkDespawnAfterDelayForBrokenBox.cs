using System.Collections;
using UnityEngine;
using Fusion;

public class NetworkDespawnAfterDelayFForBrokenBox : NetworkBehaviour
{
    [SerializeField] private float delay = 3f;
    [SerializeField] private float downwardForceDelay = 0.2f; // 幾秒後開始向下壓力
    [SerializeField] private float downwardForceMultiplier = 15f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            StartCoroutine(ApplyDownwardForceAfterDelay());
            StartCoroutine(DespawnLater());
        }
    }

private IEnumerator ApplyDownwardForceAfterDelay()
{
    yield return new WaitForSeconds(downwardForceDelay);

    if (TryGetComponent<Rigidbody>(out var rb) && rb.gameObject.activeInHierarchy)
    {
        rb.WakeUp(); // 確保沒睡著
        rb.velocity = Vector3.down * 100f; // 直接設速度
        Debug.Log($"[BrokenBox] ✅ 對 {rb.name} 設定下墜速度");
    }
    else
    {
        Debug.LogWarning("[BrokenBox] ❗ 無法施加下壓力，物件可能已被回收");
    }
}


    private IEnumerator DespawnLater()
    {
        yield return new WaitForSeconds(delay);

        if (Object != null && Object.IsValid)
        {
            Runner.Despawn(Object);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
