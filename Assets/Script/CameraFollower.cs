using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("追蹤設定")]
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.3f;
    private Vector3 _offset;
    private Vector3 _currentVelocity;

    [Header("攝影機移動邊界")]
    [Tooltip("X 軸最小值, 最大值")]
    [SerializeField] private Vector2 limitX = new Vector2(-2300f, 2000f);
    [Tooltip("Z 軸最小值, 最大值")]
    [SerializeField] private Vector2 limitZ = new Vector2(-2000f, -450f);

    private void Awake()
    {
        if (target != null)
            _offset = target.position - transform.position;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        // 1. 計算理想的相機位置（維持 offset 並做平滑）
        Vector3 desiredPos = Vector3.SmoothDamp(
            transform.position,
            target.position - _offset,
            ref _currentVelocity,
            smoothTime
        );

        // 2. clamp 到指定邊界
        desiredPos.x = Mathf.Clamp(desiredPos.x, limitX.x, limitX.y);
        desiredPos.z = Mathf.Clamp(desiredPos.z, limitZ.x, limitZ.y);

        // 3. 指派回 transform
        transform.position = desiredPos;
    }

    /// <summary>
    /// 動態切換追蹤目標
    /// </summary>
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
            _offset = target.position - transform.position;
    }
}
