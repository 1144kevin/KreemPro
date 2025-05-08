using UnityEngine;

public class VisualBullet : MonoBehaviour
{
  [Header("射速與存活時間")]
  public float speed = 30f;
  public float lifetime = 4f;

  [Header("最遠射程（可選）")]
  public float maxDistance = 100f;

  [Header("射線檢測圖層")]
  public LayerMask hitLayer;

  // 內部變數
  private Vector3 dir;
  private float timer;

  /// <summary>
  /// 初始化子彈的方向
  /// </summary>
  public void Init(Vector3 direction)
  {
    dir = direction.normalized;
    timer = 0f;
  }

  void Update()
  {
    // 計算下一個位置
    Vector3 nextPos = transform.position + dir * speed * Time.deltaTime;

    // 從上一個位置到下一個位置畫一條射線，檢測是否命中
    float travelDist = Vector3.Distance(transform.position, nextPos);
    if (Physics.Raycast(transform.position, dir, out RaycastHit hit, travelDist, hitLayer))
    {
      Debug.Log($"VisualBullet 本地命中: {hit.collider.name}");

      // 命中後立即銷毀子彈
      Destroy(gameObject);
      return;
    }

    // 沒命中就移動到下一個位置
    transform.position = nextPos;

    // 計時並檢查是否超過生命期
    timer += Time.deltaTime;
    if (timer >= lifetime)
    {
      Destroy(gameObject);
    }
  }
}
