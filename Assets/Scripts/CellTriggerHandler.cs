using UnityEngine;

public class CellTriggerHandler : MonoBehaviour
{
    [SerializeField]
    private bool setTriggersOnStart = true;

    [SerializeField]
    private bool includeInactiveObjects = false;

    private void Start()
    {
        if (setTriggersOnStart)
        {
            SetChildrenToTriggers();
        }
    }

    public void SetChildrenToTriggers()
    {
        // Get all colliders in children, including inactive if specified
        Collider[] childColliders = GetComponentsInChildren<Collider>(includeInactiveObjects);

        int triggersSet = 0;
        foreach (Collider collider in childColliders)
        {
            // Skip the parent object's collider if it exists
            if (collider.transform == transform)
                continue;

            collider.isTrigger = true;
            triggersSet++;
            Debug.Log($"Set {collider.gameObject.name} collider to trigger");
        }

        Debug.Log($"Successfully set {triggersSet} colliders to triggers");
    }

    // Public method to toggle triggers on/off
    public void ToggleChildrenTriggers(bool enableTriggers)
    {
        Collider[] childColliders = GetComponentsInChildren<Collider>(includeInactiveObjects);
        
        foreach (Collider collider in childColliders)
        {
            if (collider.transform == transform)
                continue;

            collider.isTrigger = enableTriggers;
        }
        
        Debug.Log($"Toggled {childColliders.Length} colliders to {(enableTriggers ? "triggers" : "non-triggers")}");
    }
}