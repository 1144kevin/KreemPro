using UnityEngine;

public class DieRespawn : MonoBehaviour
{
    public Vector3 respawnPosition;
    public ParticleSystem respawnedObject;

    public void DestroyAndRespawn()
    {
        transform.position = respawnPosition;
        transform.rotation = Quaternion.identity;
        respawnedObject.Play();
    }
}