using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotation : MonoBehaviour
{
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // 每秒旋轉的角度

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
