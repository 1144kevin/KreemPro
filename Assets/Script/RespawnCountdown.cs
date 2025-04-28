using UnityEngine;
using TMPro;
using Fusion;
using System.Collections;

public class RespawnCountdown : NetworkBehaviour
{
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private float extraTime = 3f;
    public float countdownTime = 3.5f;
    private float timer;
    public float RemainingTime
    {
        get { return timer; }
    }
    private Vector2 initialAnchoredPos;
    private Vector3 initialScale;
    private Color initialColor;
    private bool isCounting = false;

    private void Awake()
    {
        // 記錄倒數文字的初始狀態
        RectTransform rt = countdownText.GetComponent<RectTransform>();
        initialAnchoredPos = rt.anchoredPosition;
        initialScale = rt.localScale;
        initialColor = countdownText.color;
    }

    private void OnEnable()
    {
        timer = countdownTime;
        isCounting = true;
        UpdateCountdownText();

        RectTransform rt = countdownText.GetComponent<RectTransform>();
        rt.anchoredPosition = initialAnchoredPos;
        rt.localScale = initialScale;
        countdownText.color = initialColor; // 還原顏色
    }

    private void Update()
    {
        if (isCounting)
        {
            timer -= Time.deltaTime;
            UpdateCountdownText();

            if (timer <= 0)
            {
                isCounting = false;
            }
        }
    }

    private void UpdateCountdownText()
    {
        // 字串格式化的方式來達到保留小數點後一位
        string secondsLeft = timer.ToString("F1");
        countdownText.text = secondsLeft;
    }

    public void OnInputError()
    {
        timer += extraTime;
        isCounting = true;
        StartCoroutine(TweenCountdownText(0.5f));
    }

    IEnumerator TweenCountdownText(float duration)
    {
        RectTransform rt = countdownText.GetComponent<RectTransform>();
        if (rt == null)
            yield break;

        // 保存起始狀態
        Vector2 startPos = rt.anchoredPosition;
        Vector2 targetPos = Vector2.zero;
        Vector3 startScale = rt.localScale;
        Vector3 targetScale = startScale * 3f;
        Color startColor = countdownText.color;
        Color targetColor = new Color(1f, 0f, 0f, 1f); // 目標顏色：紅色

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            rt.anchoredPosition = Vector2.Lerp(startPos, targetPos, t);
            rt.localScale = Vector3.Lerp(startScale, targetScale, t);
            countdownText.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }
        rt.anchoredPosition = targetPos;
        rt.localScale = targetScale;
    }
}
