using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KreemLogo : MonoBehaviour
{
    public float scaleMultiplier = 1.2f; // 最大放大倍數
    public float pulseSpeed = 2f;        // 呼吸速度

    private Vector3 initialScale;

    void Start()
    {
        initialScale = transform.localScale;
    }

    void Update()
    {
        float scale = 1 + Mathf.Sin(Time.time * pulseSpeed) * (scaleMultiplier - 1);
        transform.localScale = initialScale * scale;
    }
}
