using System.Collections;
using UnityEngine;
using UnityEngine.UI;



public class FortuneWheelSpinner2 : MonoBehaviour
{
    public Image CircleBase;
    public Image[] RewardPictures;
    public GameObject Rewardpanel;
    public Image rewardFinalImage;

    [HideInInspector]
    public bool isSpinning = false;
    [HideInInspector]
    public float spinSpeed = 0;
    [HideInInspector]
    public float spinRotation = 0;
    [HideInInspector]
    public int rewardNumber = -1;


 public PlayerController playerController;
   public PlayerController playerController1;
   public PlayerController playerController2;
    public PlayerController playerController3;

    private void Start()
    {
        spinRotation = 0;
        isSpinning = false;
        rewardNumber = -1;
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.C) && !isSpinning)
    {
        Startspin();
    }
    
        if (isSpinning)
        {
            Rewardpanel.gameObject.SetActive(false);

            // 減速控制
            if (spinSpeed > 2)
            {
                spinSpeed -= 3 * Time.deltaTime;
            }
            else
            {
                spinSpeed -= 0.6f * Time.deltaTime;
            }

            spinRotation += 100 * Time.deltaTime * spinSpeed;
            CircleBase.transform.localRotation = Quaternion.Euler(0, 0, -spinRotation);

            // 停止時
            if (spinSpeed <= 0)
            {
                spinSpeed = 0;
                isSpinning = false;

                // 計算獎勵
                rewardNumber = Mathf.FloorToInt(((spinRotation % 360 + 360 + 30f) % 360) / 60) % 6;

                // 顯示獎勵面板
                rewardFinalImage.sprite = RewardPictures[rewardNumber].sprite;
                Rewardpanel.gameObject.SetActive(true);

                 // 處理獎勵
                HandleReward(); // 確保這裡調用了獎勵邏輯
            }
        }
        else
        {
            // 動態縮放
            if (rewardNumber != -1)
            {
                float scale = 1 + 0.2f * Mathf.Sin(Time.time * 10);
                RewardPictures[rewardNumber].transform.localScale = Vector3.one * scale;
            }
        }
    }

    public void Startspin()
    {
        Debug.Log("Spin");
        if (!isSpinning && rewardNumber != -1)
        {
            rewardNumber = -1; // 重置獎勵編號
            rewardFinalImage.gameObject.SetActive(true); 
        }

        if (!isSpinning && rewardNumber == -1)
        {
             spinSpeed = Random.Range(10f, 18f);
            isSpinning = true;
            Rewardpanel.gameObject.SetActive(false); // 隱藏獎勵面板
        }
    }

    private void HandleReward()
    {
        // 獎勳處理
        if (rewardNumber == 0) // 假設 element 1 是獎勳 0
        {
            if (playerController != null)
            {
                playerController.IncreaseSpeed(200); // 增加速度
                StartCoroutine(HideRewardImageAfterDelay(10f));
            
            }
        
        }
        // 可以擴展更多獎勳處理
        if (rewardNumber == 0) // 假設 element 1 是獎勳 0
        {
            if (playerController1 != null)
            {
                playerController1.IncreaseSpeed(200); // 增加速度
                StartCoroutine(HideRewardImageAfterDelay(10f));
            
            }
        
        }
        if (rewardNumber == 0) // 假設 element 1 是獎勳 0
        {
            if (playerController2 != null)
            {
                playerController2.IncreaseSpeed(200); // 增加速度
                StartCoroutine(HideRewardImageAfterDelay(10f));
            
            }
        
        }
        if (rewardNumber == 0) // 假設 element 1 是獎勳 0
        {
            if (playerController3 != null)
            {
                playerController3.IncreaseSpeed(200); // 增加速度
                StartCoroutine(HideRewardImageAfterDelay(10f));
            
            }
        
        }
    }
    private IEnumerator HideRewardImageAfterDelay(float delay)
    {
        // 等待指定的時間
        yield return new WaitForSeconds(delay);

        // 隱藏獎勳圖片
        rewardFinalImage.gameObject.SetActive(false);
    }
 
  
}


