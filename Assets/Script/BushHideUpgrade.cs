// using UnityEngine;
// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;

// public class BushHideUpgrade : MonoBehaviour
// {
//     public GameObject Bush;
//     public float fadeDuration = 1f; // 漸變持續時間
//     public float transparentAlpha = 0.3f; // 草叢完全透明時的透明度值（可自行調整）

//     private Material[] bushMaterials; // 草叢的材質列表
//     private Coroutine currentFadeCoroutine;
//     private Dictionary<GameObject, GameObject> playerCameraPairs = new Dictionary<GameObject, GameObject>();

//     private void Start()
//     {
//         // 初始化材質（克隆材質，避免影響其他物件）
//         Renderer renderer = Bush.GetComponent<Renderer>();
//         bushMaterials = renderer.materials;
//         for (int i = 0; i < bushMaterials.Length; i++)
//         {
//             bushMaterials[i] = new Material(bushMaterials[i]);
//         }
//         renderer.materials = bushMaterials;



//     }

//     private void OnTriggerEnter(Collider other)
//     {
//         // GameObject[] player1 = GameObject.FindGameObjectsWithTag("Leopard");
//         // GameObject[] player2 = GameObject.FindGameObjectsWithTag("Eagle");
//         // TagManager tagManager = FindObjectOfType<TagManager>();
//         // List<GameObject> player1 = tagManager.GetObjectsByTag("Leopard");
//         // List<GameObject> player2 = tagManager.GetObjectsByTag("Eagle");
//         // CameraFollower cameraFollower = FindObjectOfType<CameraFollower>();
//         // CameraFollower Target = player1.<CameraFollower>();
//         // foreach (GameObject player in player1)
//         // {

//         //     if(Target.target = player1.)

//         // }
//         string tag = other.gameObject.tag;
//         if (other.CompareTag("Leopard") || other.CompareTag("Eagle"))
//         {
//             switch (tag)
//             {
//                 case "Leopard":
//                     Debug.Log("Leopard in");
//                     GameObject camera = GameObject.Find("LeopardCamera");
//                     // GameObject[] taggedObjects = GameObject.FindGameObjectsWithTag("Leopard");
//                     // foreach (GameObject obj in taggedObjects)
//                     // {
//                     // string cameraName = obj.name + "Camera";
                    
//                     // }
//                     break;
//                 case "Eagle":
//                     Debug.Log("Hit an Enemy!");
//                     break;
//             }
//         }

//         {
//             // 當地玩家的攝影機進入草叢，僅改變草叢透明度
//             StartFade(transparentAlpha);
//         }
//         else if (cameraFollower.target != other.transform)
//         {
//             // 其他玩家的攝影機看到草叢，讓玩家隱藏
//             Debug.Log($"{cameraFollower.target}");
//             Debug.Log($"{other.transform}");
//             SetPlayerVisibility(other.gameObject, false);
//             Debug.Log("dismiss");
//         }

//     }

//     private void OnTriggerExit(Collider other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             CameraFollower cameraFollower = FindObjectOfType<CameraFollower>(); // 找到 CameraFollower
//             if (cameraFollower != null && cameraFollower.target == other.transform)
//             {
//                 // 當地玩家的攝影機離開草叢，恢復草叢透明度
//                 StartFade(1f);
//             }
//             else
//             {
//                 // 其他玩家的攝影機離開草叢，恢復玩家顯示
//                 SetPlayerVisibility(other.gameObject, true);
//             }
//         }
//     }

//     private void StartFade(float targetAlpha)
//     {
//         // 如果有其他漸變在進行，停止它
//         if (currentFadeCoroutine != null)
//         {
//             StopCoroutine(currentFadeCoroutine);
//         }

//         // 啟動新的漸變
//         currentFadeCoroutine = StartCoroutine(FadeMaterials(targetAlpha));
//     }

//     private IEnumerator FadeMaterials(float targetValue)
//     {
//         float startValue = bushMaterials[0].GetFloat("_Transparent"); // 獲取當前透明度
//         float elapsedTime = 0f;

//         while (elapsedTime < fadeDuration)
//         {
//             elapsedTime += Time.deltaTime;
//             float newValue = Mathf.Lerp(startValue, targetValue, elapsedTime / fadeDuration);

//             // 更新材質的 Transparent 屬性
//             foreach (Material material in bushMaterials)
//             {
//                 material.SetFloat("_Transparent", newValue);
//             }

//             yield return null; // 等待下一幀
//         }

//         // 確保最終值設置為目標值
//         foreach (Material material in bushMaterials)
//         {
//             material.SetFloat("_Transparent", targetValue);
//         }

//         currentFadeCoroutine = null; // 清除協程狀態
//     }

//     private void SetPlayerVisibility(GameObject player, bool isVisible)
//     {
//         Renderer[] renderers = player.GetComponentsInChildren<Renderer>();

//         foreach (Renderer renderer in renderers)
//         {
//             renderer.enabled = isVisible;
//         }
//     }
// }
