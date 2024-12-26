using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KTowerALTDamage : MonoBehaviour
{
    private Coroutine currentCoroutine;
    private void OnTriggerEnter(Collider collider)
    {
        if (currentCoroutine != null && collider.CompareTag("Mushroom")||collider.CompareTag("Eagle")||collider.CompareTag("Robot")||collider.CompareTag("Leopard"))
        {
            StopCoroutine(currentCoroutine); // 停止之前的協程
        }
        currentCoroutine = StartCoroutine(HandlePlayerInteraction(collider.gameObject));
    }

    private void OnTriggerExit(Collider collider)
    {
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine); // 停止協程
            currentCoroutine = null;
        }
    }
    private IEnumerator HandlePlayerInteraction(GameObject player)
    {
        Health damage=player.GetComponent<Health>();
        float stayTime = 0f;

        while (stayTime <= 1f)
        {
            yield return new WaitForSeconds(1.0f);
            if (stayTime == 1)
            {
                Debug.Log("hit");
                damage.TakeDamage(20);
                
            }
            stayTime += 1.0f;
        }
    }
}
