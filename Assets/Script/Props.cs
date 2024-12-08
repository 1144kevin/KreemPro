using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Props : MonoBehaviour
{
    [Header("Mine")]
    public GameObject mine;
    public GameObject mineVFX;
    private int mineNum = 0;
 
    [Header("Health")]
     private int hp = 0;
     [SerializeField] private float healthIncreaseAmount = 30f;
    
    [Header("Speed")]
    public GameObject speedVFX;
    private GameObject currentSpeedVFX;
    private bool isSpeedBoosted = false;
    public float speedBoostAmount = 5f;
    public float speedBoostDuration = 10f;
    private float speedBoostTimer = 0f;
    public Vector3 speedVFXOffset = new Vector3(0, 1, -1); // Adjust this in the inspector
    
    [Header("Kreem")]
    private int kreemNum = 0;

    [Header("Coins")]
    private int coinsNum = 0;


    [Header("Reference")]
    private PlayerMovement player;
    private BarSection barSection;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI CoinsText;
    [SerializeField] private TextMeshProUGUI KreemText;
    [SerializeField] private TextMeshProUGUI HpText;

    
    
    
    private void Start()
    {
        player = GetComponent<PlayerMovement>();
        barSection = GetComponent<BarSection>();
    }

    private void Update()
    {
        mineBomb();
        UpdateSpeedBoost();
        UpdateSpeedVFXPosition();
    }

    private void UpdateSpeedVFXPosition()
    {
        if (currentSpeedVFX != null)
        {
            // Calculate the VFX position using the player's transform and the offset
            Vector3 vfxPosition = transform.position + transform.rotation * speedVFXOffset;
            currentSpeedVFX.transform.position = vfxPosition;
            currentSpeedVFX.transform.rotation = transform.rotation;
        }
    }

    private void UpdateSpeedBoost()
    {
        if (isSpeedBoosted)
        {
            speedBoostTimer -= Time.deltaTime;
            
            if (speedBoostTimer <= 0)
            {
                // Remove speed boost
                player.Speed -= speedBoostAmount;
                isSpeedBoosted = false;
                
                // Destroy the speed VFX
                if (currentSpeedVFX != null)
                {
                    Destroy(currentSpeedVFX);
                }
                
                Debug.Log("Speed boost deactivated");
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("FirstAid"))
        {
            Destroy(collision.gameObject);
            hp++;
            Debug.Log(hp);
            HpText.text = "Hp: " + hp;
            barSection.IncreaseHealth(healthIncreaseAmount);

        }

        if (collision.gameObject.CompareTag("Booster"))
        {
            Destroy(collision.gameObject);
            ApplySpeedBoost();
        }

        if (collision.gameObject.CompareTag("Mine"))
        {
            collision.gameObject.SetActive(false);
            Debug.Log("u have Mine");
            mineNum++;
        }
         if (collision.gameObject.CompareTag("Kreem"))
        {
            Destroy(collision.gameObject);
            kreemNum++;
            Debug.Log(kreemNum);
            KreemText.text = "Kreem: " + kreemNum; // Updated to match naming convention
        }

    }

    private void ApplySpeedBoost()
    {
        if (!isSpeedBoosted)
        {
            player.Speed += speedBoostAmount;
            isSpeedBoosted = true;
            
            // Destroy existing VFX if there is one
            if (currentSpeedVFX != null)
            {
                Destroy(currentSpeedVFX);
            }
            
            // Create new VFX at the offset position
            currentSpeedVFX = Instantiate(speedVFX, 
                transform.position + transform.rotation * speedVFXOffset, 
                transform.rotation);
            
            speedBoostTimer = speedBoostDuration;
            Debug.Log("Speed boost activated");
        }
        else
        {
            // Reset the timer if collecting boost while already boosted
            speedBoostTimer = speedBoostDuration;
        }
    }

    private void mineBomb()
    {
        if(mineNum>0 && Input.GetKeyDown("space"))
        {
            GameObject _mine = Instantiate(mine, transform.position, transform.rotation);
            GameObject vfx = Instantiate(mineVFX, transform.position, transform.rotation);
            Destroy(_mine,5f);
            Destroy(vfx,3f);
            mineNum--;
        } 
    }
}