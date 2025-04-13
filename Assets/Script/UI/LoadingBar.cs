using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image fillImage; // 指向填滿的那個Image
    [SerializeField] private float loadingSpeed = 0.5f; // 控制進度條加速的速度

    private float currentProgress = 0f;

    void Update()
    {
        // 模擬從0到1的填滿
        if (currentProgress < 1f)
        {
            currentProgress += loadingSpeed * Time.deltaTime;
            fillImage.fillAmount = currentProgress;
        }
    }
}
