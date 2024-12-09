using UnityEngine;

public class DieRespawn : MonoBehaviour
{
    public Vector3 respawnPosition;
    public ParticleSystem respawnedObject;

    public void DestroyAndRespawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity; // 可以根據需要調整角度
        if (respawnedObject != null)
        {
            respawnedObject.Play(); // 啟動重生特效
        }
    }
}


