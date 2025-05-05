using UnityEngine;
using TMPro;
using System.Collections;

public class PlayerUIManager : MonoBehaviour
{
    [SerializeField] private GameObject respawnCanvas;
    [SerializeField] private TMP_Text kreemText;
    [SerializeField] private float startGameTime = 2.0f;

    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void LateUpdate()
    {
        if (kreemText != null)
        {
            kreemText.text = player.kreemCollect.ToString();
        }
    }

    public void RefreshRespawnUI()
    {
        if (!player.Object.HasInputAuthority) return;

        if (player.HasGameEnded)
        {
            if (respawnCanvas != null && respawnCanvas.activeSelf)
                respawnCanvas.SetActive(false);
            return;
        }

        if (player.playerHealth != null && player.playerHealth.IsDead)
        {
            if (respawnCanvas != null && !respawnCanvas.activeSelf)
                respawnCanvas.SetActive(true);
        }
        else
        {
            if (respawnCanvas != null && respawnCanvas.activeSelf)
                respawnCanvas.SetActive(false);
        }
    }

    public void InitKreemText()
    {
        var canvas = GetComponentInChildren<Canvas>(true);
        if (canvas != null)
        {
            var tmps = canvas.GetComponentsInChildren<TMP_Text>(true);
            foreach (var tmp in tmps)
            {
                if (tmp.name.Contains("Kreem"))
                {
                    kreemText = tmp;
                    Debug.Log($"[{player.Object.InputAuthority}] 綁定 KreemText：{tmp.name}");
                    break;
                }
            }

            if (kreemText == null)
                Debug.LogWarning("找不到 Kreem TMP_Text");
        }
        else
        {
            Debug.LogWarning("找不到 Canvas");
        }
    }

    public void ShowStartGameUI()
    {
        if (!player.Object.HasInputAuthority) return;
        StartCoroutine(EnableStartUI());
    }

    private IEnumerator EnableStartUI()
    {
        var ui = GameObject.Find("StartGameUI");
        if (ui != null)
        {
            ui.SetActive(true);
            ui.transform.localScale = Vector3.zero;

            LeanTween.scale(ui, Vector3.one, 0.3f).setEaseOutBack();
            yield return new WaitForSeconds(startGameTime - 0.2f);

            LeanTween.scale(ui, Vector3.zero, 0.2f).setEaseInBack();
            yield return new WaitForSeconds(0.2f);
            ui.SetActive(false);
        }
    }
}
