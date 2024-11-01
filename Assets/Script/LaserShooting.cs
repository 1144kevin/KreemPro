using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserShooting : MonoBehaviour
{
    public GameObject LaserPrefab;
    public GameObject Firepoint;
    private GameObject spwanedLaser;
    // Start is called before the first frame update
    void Start()
    {
        spwanedLaser = Instantiate(LaserPrefab,Firepoint.transform);
        DisableLaser();

        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("space")){
             EnableLaser();
        }
        if(Input.GetKey("space")){
             UpdateLaser();
        }
        if(Input.GetKeyUp("space")){
             DisableLaser();
        } 
    }
    void EnableLaser(){
        spwanedLaser.SetActive(true);
    }
    void UpdateLaser(){
        if(Firepoint!=null){
            spwanedLaser.transform.position = Firepoint.transform .position ;
        }
    }

    void DisableLaser(){
        spwanedLaser.SetActive(false);
    }
}
