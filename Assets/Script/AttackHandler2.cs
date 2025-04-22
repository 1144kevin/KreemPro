using System.Collections;
using Fusion;
using UnityEngine;

public class AttackHandler2 : NetworkBehaviour
{
    [SerializeField] private Transform CharacterTrans;
    [SerializeField] private LayerMask HitLayer;
    [SerializeField] private HitOptions HitOptions = HitOptions.IncludePhysX | HitOptions.SubtickAccuracy | HitOptions.IgnoreInputAuthority;
    [SerializeField] private int damage = 10;

    [SerializeField] private ObjectSpawner objectSpawner;

    // private void Update()
    // {
    //     Debug.Log($"🔁 Update from {gameObject.name} | ID: {GetInstanceID()}");
    // }

    public void SetSpawner(ObjectSpawner spawner)
    {
        Debug.Log($"🧩 Spawner 被設置 on {gameObject.name} | ID: {GetInstanceID()}");
        objectSpawner = spawner;
    }

    public void Attack()
    {
        Debug.Log($"🛠️ Attack() 被呼叫！ID: {GetInstanceID()} | name: {gameObject.name}", this);
        Debug.Log("Attack() 被呼叫", this);
        if (!HasStateAuthority && !HasInputAuthority)
            Debug.LogWarning("⚠️ AttackHandler 的物件不是任何權限擁有者！可能是 Editor 中的場景物件");

        if (Object == null)
            Debug.LogError("❌ NetworkObject 沒綁上！AttackHandler 無法取得 Runner");

        Debug.Log($"[Debug] Runner: {Runner}, LagComp: {Runner?.LagCompensation}, Spawner: {objectSpawner}");

        if (objectSpawner == null || Runner == null || Runner.LagCompensation == null)
        {
            Debug.LogError("❌ LagCompensation 或 Spawner 尚未初始化！");
            return;
        }

        objectSpawner.SpawnSphere();
        StartCoroutine(DespawnAfterDelay(3f));

        // 🔴 延遲補償 Raycast 設定
        Vector3 origin = CharacterTrans.position + Vector3.up * 1f; // 根據角色調整高度
        Vector3 direction = CharacterTrans.forward;
        float maxDistance = 100f;

        Debug.DrawRay(origin, direction * maxDistance, Color.red, 1f);

        // ✅ 執行 Lag Compensation Raycast
        if (Runner.LagCompensation.Raycast(
            origin,
            direction,
            maxDistance,
            Object.InputAuthority,
            out LagCompensatedHit hit,
            HitLayer,
            HitOptions))
        {
            Debug.Log("🎯 命中物體：" + hit.GameObject.name);

            if (hit.GameObject.TryGetComponent<Player>(out var hitPlayer))
            {
                Debug.Log("🎯 命中玩家：" + hitPlayer.name);
                hitPlayer.TakeDamage(damage);
            }
        }
        else
        {
            Debug.Log("❌ 沒有命中任何物體");
        }
    }

    private IEnumerator DespawnAfterDelay(float delay)
    {

        yield return new WaitForSeconds(delay);  // 等待指定秒數
        objectSpawner.DespawnAll();  // 3 秒後執行
        //Debug.Log("despawn");
    }


}
