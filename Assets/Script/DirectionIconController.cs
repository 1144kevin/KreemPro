using UnityEngine;
using UnityEngine.UI;

public class DirectionIconController : MonoBehaviour
{
    // 對應四個方向的正常與正確狀態 Sprite
    public Sprite normalUp;
    public Sprite normalDown;
    public Sprite normalLeft;
    public Sprite normalRight;

    public Sprite correctUp;
    public Sprite correctDown;
    public Sprite correctLeft;
    public Sprite correctRight;

    private Image img;

    // 紀錄目前設定的方向
    public enum Direction { Up, Down, Left, Right }
    public Direction currentDirection;

    void Awake()
    {
        img = GetComponent<Image>();
    }

    // 設定圖示為某個方向的正常狀態
    public void SetDirection(Direction dir)
    {
        currentDirection = dir;
        switch (dir)
        {
            case Direction.Up:
                img.sprite = normalUp;
                break;
            case Direction.Down:
                img.sprite = normalDown;
                break;
            case Direction.Left:
                img.sprite = normalLeft;
                break;
            case Direction.Right:
                img.sprite = normalRight;
                break;
        }
    }

    // 切換到正確狀態圖示
    public void SetCorrect()
    {
        switch (currentDirection)
        {
            case Direction.Up:
                img.sprite = correctUp;
                break;
            case Direction.Down:
                img.sprite = correctDown;
                break;
            case Direction.Left:
                img.sprite = correctLeft;
                break;
            case Direction.Right:
                img.sprite = correctRight;
                break;
        }
    }
}
