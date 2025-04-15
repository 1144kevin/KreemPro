using UnityEngine;
using UnityEngine.UI;

public class LoadingBar : MonoBehaviour
{
    [SerializeField] private Image fillBar;
    [SerializeField] private float duration = 1f;
    private float timer;

    private void Start()
    {
        timer = 0f;
    }

    private void Update()
    {
        if (timer < duration)
        {
            timer += Time.deltaTime;
            fillBar.fillAmount = timer / duration;
        }
    }
}
