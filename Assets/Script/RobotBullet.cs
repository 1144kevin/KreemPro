using UnityEngine;

public class RobotBullet : MonoBehaviour
{
    public float speed = 40f;        // 子彈速度
    public float lifeTime = 2f;     // 子彈存在的時間
    private Vector3 moveDirection;  // 子彈移動方向

    // 初始化子彈的目標方向
    public void Initialize(Vector3 targetPosition)
    {
        // 計算方向向量（目標位置 - 子彈生成位置）
        moveDirection = (targetPosition - transform.position).normalized;

        // 設定子彈自動銷毀
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        // 根據方向向量移動子彈
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet"){
            
        }
        // 銷毀子彈
        Destroy(gameObject);
    }
}