using UnityEngine;

public class CylinderMovement : MonoBehaviour
{
    public float pushForce = 10f;  // 控制後退的力度
    public float pushDuration = 2f; // 持續時間2秒
    private Rigidbody rb;
    private Vector3 pushDirection;
    private bool isPushed = false;
    private float pushEndTime;

    // 子物件上的特效（例如一個粒子系統）
    public GameObject specialEffect;  // 指向包含特效的子物件

    void Start()
    {
        // 獲取圓柱體的剛體組件
        rb = GetComponent<Rigidbody>();
        // 停止特效，確保它在開始時不運行
        if (specialEffect != null)
        {
            specialEffect.SetActive(false);  // 確保特效開始時是關閉的
        }
    }

    // 碰撞檢測
    void OnCollisionEnter(Collision collision)
    {
        // 檢查碰撞對象是否是玩家的手或其他需要觸發的對象
        if (collision.gameObject.tag == "PlayerHit")
        {
            Debug.Log("hit");
            // 計算碰撞方向（從碰撞點推開圓柱體）
            pushDirection = transform.position - collision.transform.position;
            pushDirection = pushDirection.normalized; // 正規化方向向量

            // 開始施加推力
            isPushed = true;
            specialEffect.SetActive(true);  // 啟動特效
            pushEndTime = Time.time + pushDuration; // 設定結束時間
        }
    }

    void Update()
    {
        // 如果正在被推動，並且還沒有達到推動結束的時間
        if (isPushed && Time.time <= pushEndTime)
        {
            // 持續施加力使物體相反方向移動
            rb.MovePosition(rb.position + pushDirection * pushForce * Time.deltaTime);
        }
        else if (isPushed && Time.time > pushEndTime)
        {
            // 停止推動
            isPushed = false;
            // 停止特效（如果需要，可以選擇在推動結束後停止特效）
            specialEffect.SetActive(false);  // 停止特效
        }
    }
}