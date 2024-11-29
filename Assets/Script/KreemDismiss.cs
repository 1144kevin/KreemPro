using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KreemDismiss : MonoBehaviour
{
    public GameObject Kreem;
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            // 移除物件
            Kreem.SetActive(false);
        }
    }
}
