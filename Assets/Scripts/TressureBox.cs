using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public GameObject OriginalBox;
    public GameObject BrokenBoxPrefab;
    public GameObject explodeVFX;
    public GameObject laserObject;
    public GameObject specialItem;
    public float explode_minforce;
    public float explode_maxforce;
    public float explode_radius;
       void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            Debug.Log("hit");
            Explode();
        }
    }

   void Explode()
{
    if (OriginalBox != null && BrokenBoxPrefab != null)
    {
        // Instantiate the broken box and VFX
        GameObject brokenBox = Instantiate(BrokenBoxPrefab, OriginalBox.transform.position, OriginalBox.transform.rotation);
        GameObject vfx = Instantiate(explodeVFX, OriginalBox.transform.position, OriginalBox.transform.rotation);
        GameObject item = Instantiate(specialItem, OriginalBox.transform.position, OriginalBox.transform.rotation);
        Debug.Log("Broken box instantiated");

        // Set brokenBox and its children to the "BrokenPiece" layer
        brokenBox.layer = LayerMask.NameToLayer("BrokenPiece");
        foreach (Transform child in brokenBox.transform)
        {
            child.gameObject.layer = LayerMask.NameToLayer("BrokenPiece");
        }

        // Cleanup
        Destroy(OriginalBox);
        // Destroy(brokenBox, 15f);
        Destroy(vfx, 5f);
     
    }
}



}