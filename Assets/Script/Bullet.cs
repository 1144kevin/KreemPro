// using UnityEngine;
// using TMPro; // 用於管理 UI（例如攻擊條）

// public class Bullet : MonoBehaviour
// {
//     // ----------- 子彈相關設定 -----------
//     [Header("Bullet Settings")]
//     public GameObject bullet; // 子彈的預製體
//     public GameObject muzzleFlash; // 開火時的特效預製體
//     public Transform attackPoint; // 子彈生成的位置
//     public float shootForce; // 子彈的推力（未使用）
//     public float bulletLifetime = 5f; // 子彈存在的時間，超過則銷毀
//     public float bulletSpeed = 20f; // 子彈的速度

//     // ----------- 攻擊相關設定 -----------
//     [Header("Attack Settings")]
//     public bool allowButtonHold; // 是否允許長按開火
//     public float timeBetweenShooting; // 每次射擊的間隔時間
//     public int magazineSize; // 彈夾容量
//     private int bulletsLeft; // 剩餘的子彈數量
//     public float timeBetweenBulletIncrease = 1f; // 子彈恢復的間隔時間
//     private bool shooting, readyToShoot, reloading; // 各種狀態標記
//     private bool allowInvoke = true; // 用於防止多次調用射擊方法的標記
//     private float nextBulletIncreaseTime; // 下一次子彈增加的時間

//     // ----------- UI 設定 -----------
//     [Header("UI Settings")]
//     public AttackBar attackBar; // 攻擊條（用於顯示子彈數量的 UI）

//     private void Awake()
//     {
//         // 初始化變數
//         bulletsLeft = magazineSize; // 將子彈數量設為彈夾容量
//         readyToShoot = true; // 開始時允許射擊
//         nextBulletIncreaseTime = Time.time; // 設定下一次子彈回復的初始時間
//         attackBar = GetComponentInChildren<AttackBar>(); // 找到子物件中的攻擊條 UI
//     }

//     private void Update()
//     {
//         MyInput(); // 處理玩家輸入
//         CheckBulletIncrease(); // 檢查是否需要回復子彈
//     }

//     private void MyInput()
//     {
//         // 根據是否允許長按，判斷是否進行射擊
//         shooting = allowButtonHold ? Input.GetMouseButton(0) : Input.GetMouseButtonDown(0);

//         // 當準備好射擊且沒有重裝中，並且子彈數量大於 0 時進行射擊
//         if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
//         {
//             Shoot(); // 執行射擊動作
//         }
//     }

//     private void CheckBulletIncrease()
//     {
//         // 當未射擊、未重裝且子彈未滿時，間隔一定時間回復子彈
//         if (!shooting && !reloading && bulletsLeft < magazineSize && Time.time >= nextBulletIncreaseTime)
//         {
//             // 增加子彈數量，但不超過彈夾容量
//             bulletsLeft = Mathf.Min(bulletsLeft + 1, magazineSize);
//             nextBulletIncreaseTime = Time.time + timeBetweenBulletIncrease; // 更新下一次回復時間

//             // 更新攻擊條的顯示
//             attackBar.UpdateAttackBar(bulletsLeft, magazineSize);
//         }
//     }

//     private void OnCollisionEnter(Collision collision)
//     {
//         Debug.Log("Collision detected"); // 偵測碰撞事件

//         // 如果碰撞的物件標籤是 "Target"，執行命中處理
//         if (collision.gameObject.CompareTag("Target"))
//         {
//             Debug.Log("Hit"); // 打印命中日誌
//             Destroy(gameObject); // 銷毀子彈
//         }
//     }

//     private void Shoot()
//     {
//         readyToShoot = false; // 設置為不允許射擊，等待冷卻時間

//         // 生成子彈的實例，並設置其初始位置
//         GameObject currentBullet = Instantiate(bullet, attackPoint.position, Quaternion.identity);

//         // 為子彈添加向前的推力
//         Rigidbody bulletRb = currentBullet.GetComponent<Rigidbody>();
//         bulletRb.AddForce(attackPoint.forward * bulletSpeed, ForceMode.Impulse);

//         // 生成開火特效
//         GameObject currentMuzzleFlash = Instantiate(muzzleFlash, attackPoint.position, Quaternion.identity);

//         // 設置子彈和特效的銷毀時間
//         Destroy(currentBullet, bulletLifetime);
//         Destroy(currentMuzzleFlash, 1f);

//         bulletsLeft--; // 減少子彈數量
//         nextBulletIncreaseTime = Time.time + timeBetweenBulletIncrease; // 重設子彈回復時間
//         attackBar.UpdateAttackBar(bulletsLeft, magazineSize); // 更新攻擊條顯示

//         if (allowInvoke)
//         {
//             Invoke("ResetShot", timeBetweenShooting); // 在指定時間後重置射擊狀態
//             allowInvoke = false; // 防止多次調用
//         }
//     }

//     private void ResetShot()
//     {
//         readyToShoot = true; // 恢復射擊狀態
//         allowInvoke = true; // 重置調用標記
//     }
// }
