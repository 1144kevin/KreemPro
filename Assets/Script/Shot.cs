using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Addons.Physics;

public class Shot : NetworkBehaviour
{
    [SerializeField]
    private float initialImpulse = 100f;
    [SerializeField]
    private float lifeTime = 4f;

    [Networked]
    private TickTimer lifeCoolDown { get; set; }
    [Networked]
    private NetworkBool isDestroyed { get; set; }

    private bool isDestroyedRender = false;
    private float lifeTimeAfterHit = 2f;
    private NetworkRigidbody3D rigiBody;
    private Collider collider;
    public void fireShot()
    {
        rigiBody.Rigidbody.AddForce(transform.forward * initialImpulse, ForceMode.Impulse);
        if (lifeTime > 0f)
        {
            lifeCoolDown = TickTimer.CreateFromSeconds(Runner, lifeTime);
        }
    }
    //生成可能在attackhandeler控制
    //objectspawner
    // public override void FixedUpdateNetwork()
    // {
    //     collider.enabled = isDestroyed == false;

    //     if (lifeCoolDown.IsRunning == true && lifeCoolDown.Expired(Runner) == true)
    //     {
    //         Runner.Despawn(Object);
    //     }

    // }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.rigidbody != null && Object != null)
        {
            ProcessHit();
        }
    }

    private void ProcessHit()
    {
        isDestroyed=true;

        lifeCoolDown=TickTimer.CreateFromSeconds(Runner,lifeTimeAfterHit);
        collider.enabled=false;
    }


}
