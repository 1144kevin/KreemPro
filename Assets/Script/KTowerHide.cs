using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BushHide : MonoBehaviour
{
    public GameObject Bush;
    public float fadeDuration = 1f; // 漸變持續時間
    public float transparentAlpha = 0.3f; // 草叢完全透明時的透明度值（可自行調整）

    private Material[] bushMaterials; // 草叢的材質列表
    private Coroutine currentFadeCoroutine;

    private void Start()
    {
        // 初始化材質（克隆材質，避免影響其他物件）
        Renderer renderer = Bush.GetComponent<Renderer>();
        bushMaterials = renderer.materials;
        for (int i = 0; i < bushMaterials.Length; i++)
        {
            bushMaterials[i] = new Material(bushMaterials[i]);
        }
        renderer.materials = bushMaterials;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(transparentAlpha);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartFade(1f);
        }
    }
    private void StartFade(float targetAlpha)
    {
        // 如果有其他漸變在進行，停止它
        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }

        // 啟動新的漸變
        currentFadeCoroutine = StartCoroutine(FadeMaterials(targetAlpha));
    }
    private IEnumerator FadeMaterials(float targetValue)
    {
        float startValue = bushMaterials[0].GetFloat("_Transparent"); // 獲取當前透明度
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / fadeDuration);

            // 更新材質的 Transparent 屬性
            foreach (Material material in bushMaterials)
            {
                material.SetFloat("_Transparent", newValue);
            }

            yield return null; // 等待下一幀
        }

        // 確保最終值設置為目標值
        foreach (Material material in bushMaterials)
        {
            material.SetFloat("_Transparent", targetValue);
        }

        currentFadeCoroutine = null; // 清除協程狀態
    }
}