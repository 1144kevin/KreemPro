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
    { "Leopard", new Vector3(5, 5, 5) },
    { "Eagle",   new Vector3(50, 50, 50) },
 };
  private static readonly Dictionary<string, Vector3> prefabPositions = new()
 {
    { "Leopard", new Vector3(0, 0, -10) },
    { "Eagle",   new Vector3(0.5f, 0, 0) },
 };


  public void Setup(string displayName, int score, GameObject characterPrefab)
  {
    playerNameText.text = displayName;
    scoreText.text = $"Kreem:{score}";

    foreach (Transform child in iconContainer)
      Destroy(child.gameObject);

    if (prefabScales.TryGetValue(characterPrefab.name, out var scale))
      iconContainer.localScale = scale;

    var model = Instantiate(characterPrefab, iconContainer, false);
    if (prefabPositions.TryGetValue(characterPrefab.name, out var position))
    {
      model.transform.localPosition = position;
    }
    else
    {
      model.transform.localPosition = new Vector3(0, 0, -3);
    }
    model.transform.localRotation = Quaternion.Euler(0, 210, 0);
    model.transform.localScale *= 0.8f;
  }
}
