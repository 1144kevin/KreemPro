using UnityEngine;
using UnityEngine.UI;

public class BoosterUI : MonoBehaviour
{
public Booster booster;
public Slider refillSlider;
public GameObject boostHint;
public float blinkInterval = 0.5f; // 閃爍的間隔秒數

private bool wasBoosting = false;
private float blinkTimer = 0f;
private bool blinkState = true;

void Update()
{
    if (booster == null) return;

    bool isBoosting = booster.IsBoosting();

    // 當開始加速那一幀，把滑桿設為 0
    if (booster.IsCharged() && isBoosting && !wasBoosting)
    {
        refillSlider.value = 0f;
    }
    else
    {
        float chargeRatio = booster.IsCharged()
            ? 1f
            : Mathf.Clamp01(booster.refillTimerPublic / booster.refillTimePublic);
        refillSlider.value = chargeRatio;
    }

    // 處理提示閃爍
    if (booster.IsCharged() && !isBoosting)
    {
        blinkTimer += Time.deltaTime;
        if (blinkTimer >= blinkInterval)
        {
            blinkTimer = 0f;
            blinkState = !blinkState;
        }
        boostHint.SetActive(blinkState);
    }
    else
    {
        blinkTimer = 0f;
        blinkState = true;
        boostHint.SetActive(false);
    }

    wasBoosting = isBoosting; // 更新狀態
}
}
