using UnityEngine;

public class PlayerHitted : MonoBehaviour
{
    public float pushForce;
    public float pushDuration;
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    public ParticleSystem HittedEffect;

    public int damageAmount = 20; // 每次被擊中扣的血量
    private Health playerHealth;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<Health>();

        // 停止特效，確保它在開始時不運行
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // 如果正在被推動並且撞到非預期物體時，進行反彈
        if (isPushed)
        {
            if (collision.gameObject.tag != "Bullet")
            {
                Vector3 collisionNormal = collision.contacts[0].normal; // 獲取碰撞法線方向
                pushDirection = Vector3.Reflect(pushDirection, collisionNormal); // 根據碰撞法線反射推動方向

                // 將剩餘的推力當作反彈力
                float remainingPushTime = pushEndTime - Time.time;
                pushEndTime = Time.time + remainingPushTime; // 更新反彈的結束時間
            }
            return;
        }

        // 檢查碰撞對象是否是玩家的手或其他需要觸發的對象
        if (collision.gameObject.tag == "Bullet")
        {
            Debug.Log("hit");
            // 如果存在 Health 組件，扣血
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damageAmount);
            }
            // 計算碰撞方向（從碰撞點推開圓柱體）
            pushDirection = transform.position - collision.transform.position;
            pushDirection = pushDirection.normalized; // 正規化方向向量

            // 開始施加推力
            isPushed = true;
            HittedEffect.Play();  // 啟動特效
            pushEndTime = Time.time + pushDuration; // 設定結束時間
        }
    }

    void Update()
    {
        if (isPushed && Time.time > pushEndTime)
        {
            StopPush();
        }
    }

    private void StopPush()
    {
        isPushed = false;
        if (HittedEffect != null)
        {
            HittedEffect.Stop();
        }
    }
}