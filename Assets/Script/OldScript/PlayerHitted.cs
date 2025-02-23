using UnityEngine;

public class PlayerHitted : MonoBehaviour
{
    public float pushForce = 10f;  // 控制後退的力度
    public float pushDuration = 2f; // 持續時間2秒
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;
    public ParticleSystem HittedEffect;

    public int damageAmount = 30; // 每次被擊中扣的血量
    private Health playerHealth;
    private Attack attack;

    void Start()
    {
        // 獲取圓柱體的剛體組件
        rb = GetComponent<Rigidbody>();
        playerHealth = GetComponent<Health>();

        // 停止特效，確保它在開始時不運行
        if (HittedEffect != null)
        {
            HittedEffect.Stop();  // 確保特效開始時是關閉的
        }
    }

    // 碰撞檢測
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
        if (collision.gameObject.tag == "Bullet" )
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
        // 如果正在被推動，並且還沒有達到推動結束的時間
        if (isPushed && Time.time <= pushEndTime)
        {
            // 保持 y 軸的值不變，僅更新 x 和 z 軸位置
            pushDirection.y = 0;
            Vector3 newPosition = rb.position + pushDirection * pushForce * Time.deltaTime;
            newPosition.y = rb.position.y; // 確保 y 軸位置不變

            rb.MovePosition(newPosition);
        }
        else if (isPushed && Time.time > pushEndTime)
        {
            // 停止推動
            StopPush();
        }
    }

    // 停止推動的方法
    private void StopPush()
    {
        isPushed = false;
        if (HittedEffect != null)
        {
            HittedEffect.Stop();  // 停止特效
        }
    }
}
