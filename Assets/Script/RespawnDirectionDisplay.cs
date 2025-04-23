using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine.InputSystem;

public class RespawnDirectionDisplay : NetworkBehaviour
{
    public GameObject directionIconPrefab;
    public RectTransform iconContainer;
    public enum Direction { Up, Down, Left, Right }
    public int totalIcons = 8;
    private List<GameObject> iconInstances = new List<GameObject>();
    private List<Direction> sequence = new List<Direction>();
    private int currentIndex = 0;
    // 是否啟用輸入檢查
    private bool inputActive = true;
    public RectTransform countdownTextRect;
    [SerializeField] private PlayerRespawn playerRespawn;
    [SerializeField] private Player player;
    [SerializeField] private RespawnCountdown respawnCountdown;
    private bool errorTriggered = false;

    private void OnEnable()
    {
        ClearIcons();
        GenerateRandomIcons();
        currentIndex = 0;
        inputActive = true;
        errorTriggered = false;
    }

    void GenerateRandomIcons()
    {
        sequence.Clear();
        for (int i = 0; i < totalIcons; i++)
        {
            int rand = Random.Range(0, 4);
            Direction dir = (Direction)rand;
            sequence.Add(dir);

            GameObject icon = Instantiate(directionIconPrefab, iconContainer);

            DirectionIconController iconCtrl = icon.GetComponent<DirectionIconController>();
            if (iconCtrl != null)
            {
                iconCtrl.SetDirection((DirectionIconController.Direction)rand);
            }
            iconInstances.Add(icon);
        }
    }
    IEnumerator ShakeAndClear(GameObject icon)
    {
        RectTransform rt = icon.GetComponent<RectTransform>();
        if (rt == null)
            yield break;

        // 保存原始位置
        Vector3 originalPos = rt.localPosition;
        float shakeDuration = 0.3f; // 晃動持續時間
        float elapsed = 0f;
        float shakeMagnitude = 6f; // 晃動幅度，可根據需求調整

        // 晃動動畫
        while (elapsed < shakeDuration)
        {
            elapsed += Time.deltaTime;
            float offsetX = Random.Range(-shakeMagnitude, shakeMagnitude);
            float offsetY = Random.Range(-shakeMagnitude, shakeMagnitude);
            rt.localPosition = originalPos + new Vector3(offsetX, offsetY, 0f);
            yield return null;
        }
        rt.localPosition = originalPos;

        ClearIcons();
        inputActive = false;
        if (respawnCountdown != null)
        {
            respawnCountdown.OnInputError();
        }
    }


    void ClearIcons()
    {
        foreach (Transform child in iconContainer)
        {
            Destroy(child.gameObject);
        }
        iconInstances.Clear();
    }

    private void Update()
    {
        if (respawnCountdown != null && respawnCountdown.RemainingTime <= 0f)
        {
            if (currentIndex < totalIcons)
            {
                if (!errorTriggered)
                {
                    Debug.Log("倒數時間結束，未完成正確輸入，視為錯誤");
                    ClearIcons();
                    inputActive = false;
                    respawnCountdown.OnInputError();
                    errorTriggered = true;
                    return;
                }
                else
                {
                    Debug.Log("延長時間後倒數歸零，自動復活");
                    if (Object.HasInputAuthority)
                    {
                        playerRespawn.RpcRequestRespawn();
                        player.RpcDisableCameraClampClient();
                    }
                    return;
                }
            }
            else
            {
                Debug.Log("倒數時間結束，已正確輸入，觸發復活");
                if (Object.HasInputAuthority)
                {
                    playerRespawn.RpcRequestRespawn();
                    player.RpcDisableCameraClampClient();
                }
                return;
            }
        }

        if (!inputActive) return;

        if ((Gamepad.current?.dpad.up.wasPressedThisFrame ?? false) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            ProcessInput(Direction.Up);
        }
        else if ((Gamepad.current?.dpad.down.wasPressedThisFrame ?? false) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            ProcessInput(Direction.Down);
        }
        else if ((Gamepad.current?.dpad.left.wasPressedThisFrame ?? false) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            ProcessInput(Direction.Left);
        }
        else if ((Gamepad.current?.dpad.right.wasPressedThisFrame ?? false) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            ProcessInput(Direction.Right);
        }

        if (currentIndex >= sequence.Count)
        {
            Debug.Log("所有方向輸入正確，提前復活！");
            inputActive = false;
            if (Object.HasInputAuthority)
            {
                playerRespawn.RpcRequestRespawn();
                player.RpcDisableCameraClampClient();
            }
        }

        if (currentIndex >= sequence.Count)
        {
            Debug.Log("所有方向輸入正確，提前復活！");
            inputActive = false;
            if (Object.HasInputAuthority)
            {
                playerRespawn.RpcRequestRespawn();
                player.RpcDisableCameraClampClient();
            }
        }
    }



    // 處理玩家的方向輸入
    void ProcessInput(Direction inputDir)
    {
        // 檢查當前輸入是否與序列中預期的方向一致
        if (currentIndex < sequence.Count && inputDir == sequence[currentIndex])
        {
            DirectionIconController iconCtrl = iconInstances[currentIndex].GetComponent<DirectionIconController>();
            if (iconCtrl != null)
            {
                iconCtrl.SetCorrect();
            }
            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                Debug.Log("所有方向輸入正確，提前復活！");
                inputActive = false;

                if (Object.HasInputAuthority)
                {
                    playerRespawn.RpcRequestRespawn();
                    player.RpcDisableCameraClampClient();
                }
            }
        }
        else
        {
            if (!errorTriggered)
            {
                Debug.Log("輸入錯誤，開始晃動錯誤圖示...");
                errorTriggered = true;
                StartCoroutine(ShakeAndClear(iconInstances[currentIndex]));
            }
        }
    }
}
