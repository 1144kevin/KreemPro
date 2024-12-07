using UnityEngine;

public class DieRespawn : MonoBehaviour
{
    public Vector3 respawnPosition; // 指定重新生成的位置
    public ParticleSystem respawnedObject;

    void Start()
    {
       
    }
    private void Update()
    {
        // 檢測是否按下 K 鍵
        if (Input.GetKeyDown(KeyCode.K))
        {
            DestroyAndRespawn();
        }
    }

    // 用於銷毀物件並重新生成的方法，可供其他腳本調用
    public void DestroyAndRespawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        respawnedObject.Play();  // 啟動特效
    }

}

