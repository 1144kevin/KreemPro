using UnityEngine;

[ExecuteAlways]
public class CameraFollower : MonoBehaviour
{
    [Header("跟隨目標 (自動設為父物件)")]
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset = new Vector3(0, -800, 500);
    [SerializeField] private float smoothTime = 0.2f;

    [Header("邊界限制 (自動偵測)")]
    [SerializeField] private GameObject mapBoundsObject;
    private float mapMinX, mapMaxX, mapMinZ, mapMaxZ;

    [Header("邊界緩衝距離")]
    [SerializeField] private float xThreshold = 400f;
    [SerializeField] private float zThreshold = 200f;

    private Vector3 currentVelocity = Vector3.zero;
    private float factorX = 1f, factorZ = 1f;

    private void Start()
    {
        if (target == null && transform.parent != null)
            target = transform.parent;

        if (mapBoundsObject != null)
        {
            Bounds bounds = new Bounds();
            bool found = false;

            // 先從自身抓 Renderer / Collider
            if (mapBoundsObject.TryGetComponent(out Renderer renderer))
            {
                bounds = renderer.bounds;
                found = true;
            }
            else if (mapBoundsObject.TryGetComponent(out Collider collider))
            {
                bounds = collider.bounds;
                found = true;
            }
            else
            {
                // 再從子物件中找 Renderer / Collider
                Renderer childRenderer = mapBoundsObject.GetComponentInChildren<Renderer>();
                Collider childCollider = mapBoundsObject.GetComponentInChildren<Collider>();

                if (childRenderer != null)
                {
                    bounds = childRenderer.bounds;
                    found = true;
                }
                else if (childCollider != null)
                {
                    bounds = childCollider.bounds;
                    found = true;
                }
            }

            if (found)
            {
                mapMinX = bounds.min.x;
                mapMaxX = bounds.max.x;
                mapMinZ = bounds.min.z;
                mapMaxZ = bounds.max.z;
                Debug.Log($"✅ 自動偵測地圖邊界: X({mapMinX} ~ {mapMaxX}), Z({mapMinZ} ~ {mapMaxZ})");
            }
            else
            {
                Debug.LogError("❗ mapBoundsObject 必須有 Renderer 或 Collider（本體或子物件）！");
            }
        }
    }

private void LateUpdate()
{
    if (target == null) return;

    Vector3 desiredPosition = target.position - offset;

    // 是否允許在各軸向上跟隨（若在黃色區域內，就禁用）
    bool followX = !IsNearBoundary(target.position.x, mapMinX, mapMaxX, xThreshold);
    bool followZ = !IsNearBoundary(target.position.z, mapMinZ, mapMaxZ, zThreshold, ignoreMin: true); // ← 忽略 Z 最小邊界


    Vector3 adjustedTarget = new Vector3(
        followX ? desiredPosition.x : transform.position.x,
        desiredPosition.y,
        followZ ? desiredPosition.z : transform.position.z
    );

    transform.position = Vector3.SmoothDamp(transform.position, adjustedTarget, ref currentVelocity, smoothTime);
}


    private float CalculateFollowFactor(float pos, float min, float max, float threshold)
    {
        if (pos < min + threshold)
            return Mathf.Clamp01((pos - min) / threshold);
        else if (pos > max - threshold)
            return Mathf.Clamp01((max - pos) / threshold);
        else
            return 1f;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    private bool IsNearBoundary(float pos, float min, float max, float threshold, bool ignoreMin = false, bool ignoreMax = false)
    {
        bool nearMin = !ignoreMin && (pos - min < threshold);
        bool nearMax = !ignoreMax && (max - pos < threshold);
        return nearMin || nearMax;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        // 可視化邊界區域
        Gizmos.color = new Color(0f, 1f, 0f, 0.2f);
        Gizmos.DrawCube(new Vector3((mapMinX + mapMaxX) / 2f, 0, (mapMinZ + mapMaxZ) / 2f),
                        new Vector3(Mathf.Abs(mapMaxX - mapMinX), 1f, Mathf.Abs(mapMaxZ - mapMinZ)));

        // 可視化 Threshold 緩衝區
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.2f);

        // 左右 X 緩衝
        Gizmos.DrawCube(new Vector3(mapMinX + xThreshold / 2f, 0, (mapMinZ + mapMaxZ) / 2f),
                        new Vector3(xThreshold, 1f, Mathf.Abs(mapMaxZ - mapMinZ)));

        Gizmos.DrawCube(new Vector3(mapMaxX - xThreshold / 2f, 0, (mapMinZ + mapMaxZ) / 2f),
                        new Vector3(xThreshold, 1f, Mathf.Abs(mapMaxZ - mapMinZ)));

        // 上下 Z 緩衝
        Gizmos.DrawCube(new Vector3((mapMinX + mapMaxX) / 2f, 0, mapMinZ + zThreshold / 2f),
                        new Vector3(Mathf.Abs(mapMaxX - mapMinX), 1f, zThreshold));

        Gizmos.DrawCube(new Vector3((mapMinX + mapMaxX) / 2f, 0, mapMaxZ - zThreshold / 2f),
                        new Vector3(Mathf.Abs(mapMaxX - mapMinX), 1f, zThreshold));
    }
#endif
}
