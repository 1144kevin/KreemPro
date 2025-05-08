using System.Collections;
using UnityEngine;
using Fusion;

public class NetworkDespawnAfterDelayFForBrokenBox : NetworkBehaviour
{
    [Header("一般設定")]
    [SerializeField] private float delay = 3f;

    [Header("向下壓力設定")]
    private float downwardForceDelay = 0.5f;
    private float downwardForceMultiplier = 300f;

    [Header("爆破力設定")]
    private float explosionForce = 20000f;
    private Vector2 upwardRange = new Vector2(200f, 300f);
    private float outwardMultiplier = 250f;

    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            // ApplyExplosionForce();
            // StartCoroutine(ApplyDownwardForceAfterDelay());
            StartCoroutine(DespawnLater());
        }
    }

    private void ApplyExplosionForce()
    {
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.mass = 1.5f;
            rb.drag = 0.1f;
            rb.angularDrag = 0.2f;
            rb.useGravity = true;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

            Vector3 randomDir = Random.insideUnitSphere;
            randomDir.y = Random.Range(upwardRange.x, upwardRange.y);
            randomDir.x *= outwardMultiplier;
            randomDir.z *= outwardMultiplier;

            rb.AddForce(randomDir * explosionForce, ForceMode.Impulse);
            rb.AddTorque(Random.insideUnitSphere * explosionForce * 0.5f, ForceMode.Impulse);

            Debug.Log($"[BrokenBox] 💥 爆破向外施力完成：{rb.name}");
        }
    }

    private IEnumerator ApplyDownwardForceAfterDelay()
    {
        yield return new WaitForSeconds(downwardForceDelay);

        if (TryGetComponent<Rigidbody>(out var rb) && rb.gameObject.activeInHierarchy)
        {
            rb.WakeUp();
            rb.velocity = Vector3.down * downwardForceMultiplier;
            Debug.Log($"[BrokenBox] ⬇ 向下壓力套用：{rb.name}");
        }
        else
        {
            Debug.LogWarning("[BrokenBox] ❗ 無法套用向下壓力，物件可能已回收");
        }
    }

    private IEnumerator DespawnLater()
    {
        yield return new WaitForSeconds(delay);

        if (Object != null && Object.IsValid)
        {
            Debug.Log("[BrokenBox] 🧹 Despawn 執行");
            Runner.Despawn(Object);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

