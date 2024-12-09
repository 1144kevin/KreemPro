using UnityEngine;

public class DieRespawn : MonoBehaviour
{
    public Vector3 respawnPosition;
    public ParticleSystem respawnedObject;

    void Start()
    {

    }
    private void Update()
    {
        // // 檢測是否按下 K 鍵
        // if (Input.GetKeyDown(KeyCode.K))
        // {
        //     DestroyAndRespawn();
        // }
    }

    // 用於銷毀物件並重新生成的方法，可供其他腳本調用
    public void DestroyAndRespawn()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            transform.position = respawnPosition;
            transform.rotation = Quaternion.identity;
            respawnedObject.Play();  // 啟動特效
        }
    }
}