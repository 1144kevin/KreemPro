using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingEntryUI : MonoBehaviour
{
  [SerializeField] private TMP_Text playerNameText;
  [SerializeField] private TMP_Text scoreText;
  [SerializeField] private Transform iconContainer;

  // 放在 RankingEntryUI 類別最上面
  private static readonly Dictionary<string, Vector3> prefabScales = new()
  {
    { "Simmons", new Vector3(5, 5, 5) },
    { "Kuzma",   new Vector3(50, 50, 50) },
  };

  private static readonly Dictionary<string, Vector3> prefabPositions = new()
  {
    { "Simmons", new Vector3(0, 0, -10) },
    { "Kuzma",   new Vector3(0, 0, -8) },
    { "Kendall", new Vector3(0, 0, 0) },
  };

  // 新增 prefabRotations 字典
  private static readonly Dictionary<string, Vector3> prefabRotations = new()
  {
    { "Booker", new Vector3(0, 30, 0) },
    { "Simmons", new Vector3(0, 180, 0) },
    { "kendall", new Vector3(0, 180, 0) },

  };

  public void Setup(string displayName, int score, GameObject characterPrefab, bool isWinner)
  {
    playerNameText.text = displayName;
    scoreText.text = $"Kreem:{score}";

    foreach (Transform child in iconContainer)
      Destroy(child.gameObject);

    if (prefabScales.TryGetValue(characterPrefab.name, out var scale))
      iconContainer.localScale = scale;

    var model = Instantiate(characterPrefab, iconContainer, false);

    if (prefabPositions.TryGetValue(characterPrefab.name, out var position))
      model.transform.localPosition = position;
    else
      model.transform.localPosition = new Vector3(0, 0, -3);

    // 使用 prefabRotations 設定旋轉
    if (prefabRotations.TryGetValue(characterPrefab.name, out var rotation))
      model.transform.localRotation = Quaternion.Euler(rotation);
    else
      model.transform.localRotation = Quaternion.Euler(0, 210, 0); // 預設值

    model.transform.localScale *= 0.8f;

    // 播放動畫
    var animator = model.GetComponent<Animator>();
    if (animator != null)
    {
        if (isWinner)
                animator.Play("win");
            else
                animator.Play("lose");
    }
  }
}
