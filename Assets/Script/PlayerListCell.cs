using UnityEngine;
using UnityEngine.UI;

public class PlayerListCell : MonoBehaviour
{
    [SerializeField]
    private Text playerNameTxt;

    [SerializeField] private GameObject[] avatarImage;

    public void SetPlayerName(string playerName)
    {
        playerNameTxt.text = playerName;
    }

    public void SetPlayerAvatar(int characterIndex)
    {
        for (int i = 0; i < avatarImage.Length; i++)
        {
            avatarImage[i].SetActive(false);
        }
        avatarImage[characterIndex].SetActive(true);
    }

}
