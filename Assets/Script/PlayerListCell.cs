using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerListCell : MonoBehaviour
{
    [SerializeField]
    private Text playerNameTxt;

    public void SetPlayerName(string playerName)
    {
        playerNameTxt.text = playerName;
    }

}
