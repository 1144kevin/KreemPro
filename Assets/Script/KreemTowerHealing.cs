using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SphereCollider))]
public class KreemTowerHealing : MonoBehaviour
{
  [Header("回血設定")]
  [SerializeField] private int healAmount = 5;
  [SerializeField] private float healInterval = 3f;

  // 紀錄每個玩家在範圍內的累積時間
  private Dictionary<Player, float> healTimers = new Dictionary<Player, float>();

  private void Reset()
  {
    var col = GetComponent<SphereCollider>();
    col.isTrigger = true;
  }

  private void OnTriggerEnter(Collider other)
  {
    var player = other.GetComponentInParent<Player>();
    if (player == null || !player.Object.HasStateAuthority)
      return;

    // **只要已經記錄過，就不再初始化**
    if (healTimers.ContainsKey(player))
      return;

    healTimers[player] = 0f;
    Debug.Log("第一次進入範圍，只印一次");
  }

  private void OnTriggerStay(Collider other)
  {
    var player = other.GetComponentInParent<Player>();
    if (player == null || !player.Object.HasStateAuthority)
      return;

    // 確保已經有初始化過
    if (!healTimers.ContainsKey(player))
      healTimers[player] = 0f;

    healTimers[player] += Time.deltaTime;
    Debug.Log("停留中，每幀都會呼叫 Stay");

    if (healTimers[player] >= healInterval)
    {
      // player.Heal(healAmount);
      healTimers[player] = 0f;
      Debug.Log("每 3 秒回血一次");
    }
  }

  private void OnTriggerExit(Collider other)
  {
    var player = other.GetComponentInParent<Player>();
    if (player == null)
      return;

    healTimers.Remove(player);
    Debug.Log("離開範圍，計時重置");
  }
}
