using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemCollector : MonoBehaviour
{
    private int coins = 0;
 
    [SerializeField] private TextMeshProUGUI CoinsText; // Corrected type to TextMeshProUGUI

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            coins++;
            Debug.Log(coins);
            CoinsText.text = "Coins: " + coins; // Updated to match naming convention
        }

         if (collision.gameObject.CompareTag("FirstAid"))
        {
            Destroy(collision.gameObject);
          
            Debug.Log(coins);
            CoinsText.text = "Coins: " + coins; // Updated to match naming convention
        }
    }
}
