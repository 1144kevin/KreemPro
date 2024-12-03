using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FortuneWheelSpinner : MonoBehaviour
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


    private void Start()
    {
        spinRotation = 0;
        isSpinning = false;
        rewardNumber = -1;
    }

    private void Update()
    {
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
        if (!isSpinning && rewardNumber != -1)
        {
            rewardNumber = -1; // 重置獎勵編號
        }

        if (!isSpinning && rewardNumber == -1)
        {
             spinSpeed = Random.Range(10f, 18f);
            isSpinning = true;
            Rewardpanel.gameObject.SetActive(false); // 隱藏獎勵面板
        }
    }
}
