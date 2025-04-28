using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Shot : NetworkBehaviour
{
    [SerializeField] private float speed = 30f;
    [SerializeField] private float lifetime = 4f;
    [SerializeField] private ObjectSpawner objectSpawner;

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
        if (isFlying)
        {
            transform.position += shootDirection * speed * Runner.DeltaTime;
        }

        if (lifeTimer.Expired(Runner))
        {
            isFlying = false;
            Runner.Despawn(Object);
        }
    }
}
