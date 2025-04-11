using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListCell : MonoBehaviour
{
    [SerializeField]
    private Text playerNameTxt;

    [SerializeField]
    private Image playerImage;

    public void SetPlayerName(string playerName)
    {
        playerNameTxt.text = playerName;
    }
    public void SetPlayerImage(Sprite characterSprites)
    {
      if (characterSprites != null)
        {
            playerImage.sprite = characterSprites;
            playerImage.gameObject.SetActive(true);
        }
        else
        {
            playerImage.gameObject.SetActive(false);
        }
    }

}
