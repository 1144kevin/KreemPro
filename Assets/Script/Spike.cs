using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : MonoBehaviour
{
    public void Shoot()
    {
        if (this.transform.localPosition.y < 0f)
        {
            StartCoroutine(_Shoot());
        }

    }
    IEnumerator _Shoot()
    {
        Debug.Log("spike up");
        float delay = Random.Range(0f, 0.25f);
        yield return new WaitForSeconds(delay);
        this.transform.localPosition += (Vector3.up * 40f);

    }
    public void Retract()
    {
        if (this.transform.localPosition.y > 0f)
        {
            StartCoroutine(_Retract());
        }
    }
    IEnumerator _Retract()
    {
        Debug.Log("spike down");
        float delay = Random.Range(0f, 0.25f);
        yield return new WaitForSeconds(delay);
        this.transform.localPosition -= (Vector3.up * 40f);

    }
}
