using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class clickable : MonoBehaviour
{
    public float alphaThreshold=0.1f;
    void Start()
    {
        this.GetComponent<Image>().alphaHitTestMinimumThreshold=alphaThreshold;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
