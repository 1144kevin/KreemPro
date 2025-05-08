using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Shot : NetworkBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private float maxDistance = 100f;
    [SerializeField] private LayerMask hitLayer;
    [Networked] private TickTimer lifeTimer { get; set; }

    private Vector3 shootDirection;
    private bool isFlying = false;

    public void Fire(Vector3 direction)
    {
        shootDirection = direction.normalized;
        isFlying = true;

        if (lifetime > 0f)
        {
            lifeTimer = TickTimer.CreateFromSeconds(Runner, lifetime);
        }
    }

    public override void FixedUpdateNetwork()
    {
        // 如果還沒飛、LifeTimer 也還沒設定，就不碰它
        if (!isFlying)
            return;

            transform.position += shootDirection * speed * Runner.DeltaTime;

        // Raycast from current to next position to detect hit
        if (Physics.Raycast(transform.position, shootDirection, out RaycastHit hit, speed * Runner.DeltaTime, hitLayer))
        {
            Debug.Log("Shot hit: " + hit.collider.name);

            // 處理命中物件
            // if (hit.collider.TryGetComponent(out Player target))
            // {
            //     target.TakeDamage(10); // 你可以改這數字或改成變數
            // }

            // 停止飛行並回收
            isFlying = false;
            Runner.Despawn(Object);
            return;
        }

        if (lifeTimer.Expired(Runner))
        {
            isFlying = false;
            Runner.Despawn(Object);
        }
    }
}
