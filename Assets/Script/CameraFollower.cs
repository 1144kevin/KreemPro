using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    [Header("追蹤設定")]
    [SerializeField] private Transform target;
    [Tooltip("不受限時的跟隨平滑時間")]
    [SerializeField] private float followSmoothTime = 0f;
    [Tooltip("剛啟用 clamp 時的中間過渡平滑時間")]
    [SerializeField] private float middleSmoothTime = 1f;
    [Tooltip("中間過渡持續的秒數")]
    [SerializeField] private float middleDuration = 2f;
    [Tooltip("啟用邊界後的過渡平滑時間")]
    [SerializeField] private float clampSmoothTime = 0f;

    private Vector3 _offset;
    private Vector3 _currentVelocity;

    [Header("攝影機移動邊界")]
    [Tooltip("X 軸最小值, 最大值")]
    [SerializeField] private Vector2 limitX = new Vector2(300f, 2200f);
    [Tooltip("Z 軸最小值, 最大值")]
    [SerializeField] private Vector2 limitZ = new Vector2(-4000f, -750f);

    private bool isCameraClamped = false;
    private bool _prevClamped = false;
    private float _clampStartTime = -Mathf.Infinity;

    private void Awake()
    {
        if (target != null)
        {
            _offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 rawDesired = target.position + _offset;

        if (isCameraClamped && !_prevClamped)
        {
            _clampStartTime = Time.time;
        }

        Vector3 desired = rawDesired;
        if (isCameraClamped)
        {
            desired.x = Mathf.Clamp(desired.x, limitX.x, limitX.y);
            desired.z = Mathf.Clamp(desired.z, limitZ.x, limitZ.y);
        }

        float smoothTime;
        if (!isCameraClamped)
        {
            smoothTime = clampSmoothTime;
        }
        else
        {
            float elapsed = Time.time - _clampStartTime;
            if (elapsed < middleDuration)
            {
                float t = elapsed / middleDuration;
                smoothTime = Mathf.Lerp(middleSmoothTime, followSmoothTime, t);
            }
            else
            {
                smoothTime = followSmoothTime;
            }
        }

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desired,
            ref _currentVelocity,
            smoothTime
        );

        _prevClamped = isCameraClamped;
    }
    public void DisableCameraClamp()
    {
        isCameraClamped = false;
        _prevClamped = false;
        _clampStartTime = -Mathf.Infinity;
        _currentVelocity = Vector3.zero; // 同時重置平滑速度
    }

    public void EnableCameraClamp()
    {
        isCameraClamped = true;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target == null) return;
        _offset = transform.position - target.position;
        DisableCameraClamp();
        transform.position = target.position + _offset;
    }
}
