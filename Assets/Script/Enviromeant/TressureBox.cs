using System.Collections;
using UnityEngine;

public class TreasureBox : MonoBehaviour
{
    public GameObject OriginalBox;
    public GameObject BrokenBoxPrefab;
    // public GameObject explodeVFX;
    // public GameObject laserObject;
    public GameObject specialItem; public float coinForce; // 控制 specialItem 彈力效果
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Explode();
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PlayerHit"))
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
            // GameObject vfx = Instantiate(explodeVFX, OriginalBox.transform.position, OriginalBox.transform.rotation);
            // Instantiate the special item and apply force
            GameObject item = Instantiate(specialItem, OriginalBox.transform.position + new Vector3(0, 50, 0), OriginalBox.transform.rotation);
            Rigidbody itemRb = item.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                // Apply upward and random force to the item
                Vector3 forceDirection = Vector3.up + new Vector3(Random.Range(-0.5f, 0.5f), 0, Random.Range(-0.5f, 0.5f));
                itemRb.AddForce(forceDirection.normalized * coinForce, ForceMode.Impulse);
            }
            // Set brokenBox and its children to the "BrokenPiece" layer
            brokenBox.layer = LayerMask.NameToLayer("BrokenPiece");
            foreach (Transform child in brokenBox.transform)
            {
                child.gameObject.layer = LayerMask.NameToLayer("BrokenPiece");
            }

            // Cleanup
            Destroy(OriginalBox);
            Destroy(brokenBox, 3f);
            // Destroy(vfx, 5f);

        }
    }
}