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

    // ✅ 將攻擊條件封裝為 CanAttack 屬性供 Player 使用
    public bool CanAttack => Runner != null && Runner.IsRunning && Runner.LagCompensation != null && objectSpawner != null;

private void Start()
{
     Debug.Log($"[AttackHandler2.Start] 🧠 Runner 是否有值？{(Runner != null)} | Mode: {Runner?.Mode}");
    Debug.Log($"[AttackHandler2.Start] 🧠 Runner: {Runner}, LagComp: {Runner?.LagCompensation}, Mode: {Runner?.Mode}, IsRunning: {Runner?.IsRunning}");
}

    public string SpawnerStatus()
    {
        return objectSpawner != null ? objectSpawner.name : "null";
    }
    public void SetSpawner(ObjectSpawner spawner)
    {
        Debug.Log($"🧩 Spawner 被設置 on {gameObject.name} | ID: {GetInstanceID()}");
        objectSpawner = spawner;
    }
    

    public void Attack()
    {
        Debug.Log($"[攻擊時] 是否是 Spawn 物件？Object: {Object}, HasStateAuthority: {HasStateAuthority}, HasInputAuthority: {HasInputAuthority}");
        Debug.Log($"🛠️ Attack() 被呼叫！ID: {GetInstanceID()} | name: {gameObject.name}", this);

        if (!HasStateAuthority && !HasInputAuthority)
            Debug.LogWarning("⚠️ AttackHandler 的物件不是任何權限擁有者！可能是 Editor 中的場景物件");

        if (Object == null)
            Debug.LogError("❌ NetworkObject 沒綁上！AttackHandler 無法取得 Runner");

        Debug.Log($"[Debug] Runner: {Runner}, LagComp: {Runner?.LagCompensation}, Spawner: {objectSpawner}");

        if (!CanAttack)
        {
            Debug.LogError("❌ LagCompensation 或 Spawner 尚未初始化！");
            return;
        }

        objectSpawner.SpawnSphere();
        StartCoroutine(DespawnAfterDelay(3f));

        Vector3 origin = CharacterTrans.position + Vector3.up * 1f;
        Vector3 direction = CharacterTrans.forward;
        float maxDistance = 100f;

        Debug.DrawRay(origin, direction * maxDistance, Color.red, 1f);

        
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
        yield return new WaitForSeconds(delay);
        objectSpawner.DespawnAll();
    }
    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RequestAttackRpc()
    {
        Debug.Log("📥 Host 收到攻擊 RPC，執行 Attack()");
        Attack();
    }

}
