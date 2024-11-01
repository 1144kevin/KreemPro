using UnityEngine;

public class Aiming : MonoBehaviour
{
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float aimSpeed = 10f;
    [SerializeField] private float mouseMovementThreshold = 0.1f;
    [SerializeField] private float lineLength = 5f; // Length of the aiming line
    [SerializeField] private Color lineColor = Color.red; // Color of the aiming line
    
    private Camera mainCamera;
    private Transform currentTarget;
    private bool shooting;
    private bool targetDetection;
    private Vector2 lastMousePosition;
    private bool isMouseMoving;
    private bool forceManualAim;
    private LineRenderer lineRenderer; // Added line renderer component

    private void Start()
    {
        mainCamera = Camera.main;
        lastMousePosition = Input.mousePosition;
        
        // Setup line renderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.positionCount = 2;
    }

    private void Update()
    {
        shooting = Input.GetMouseButton(0);
        CheckMouseMovement();
        
        if (Input.GetMouseButtonUp(0))
        {
            forceManualAim = false;
        }
        
        if (isMouseMoving && shooting)
        {
            forceManualAim = true;
        }
        
        DetectTargets();
        
        if (targetDetection && shooting && currentTarget != null && !forceManualAim)
        {
            AutoAim();
            UpdateAimingLine(currentTarget.position);
        }
        else
        {
            ManualAim();
            var (success, position) = GetMousePosition();
            if (success)
            {
                UpdateAimingLine(position);
            }
        }
    }

    private void UpdateAimingLine(Vector3 targetPosition)
    {
        Vector3 direction = (targetPosition - transform.position).normalized;
        Vector3 lineStart = transform.position;
        Vector3 lineEnd = lineStart + direction * lineLength;
        
        // Update line renderer positions
        lineRenderer.SetPosition(0, lineStart);
        lineRenderer.SetPosition(1, lineEnd);
    }

    private void CheckMouseMovement()
    {
        Vector2 currentMousePosition = Input.mousePosition;
        float mouseMovement = Vector2.Distance(currentMousePosition, lastMousePosition);
        isMouseMoving = mouseMovement > mouseMovementThreshold;
        lastMousePosition = currentMousePosition;
    }

    private void DetectTargets()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, detectionRadius, targetMask);
        
        if (hitColliders.Length > 0)
        {
            float closestDistance = Mathf.Infinity;
            Transform closestTarget = null;
            
            foreach (var hitCollider in hitColliders)
            {
                float distance = Vector3.Distance(transform.position, hitCollider.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestTarget = hitCollider.transform;
                }
            }
            
            currentTarget = closestTarget;
            targetDetection = true;
        }
        else
        {
            currentTarget = null;
            targetDetection = false;
        }
    }

    private void AutoAim()
    {
        if (currentTarget == null) return;

        Vector3 direction = currentTarget.position - transform.position;
        direction.y = 0;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * aimSpeed);
    }

    private void ManualAim()
    {
        var (success, position) = GetMousePosition();
        if (success)
        {
            var direction = position - transform.position;
            direction.y = 0;
            transform.forward = direction;
        }
    }

    private (bool success, Vector3 position) GetMousePosition()
    {
        var ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, groundMask))
        {
            return (success: true, position: hitInfo.point);
        }
        else
        {
            return (success: false, position: Vector3.zero);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}