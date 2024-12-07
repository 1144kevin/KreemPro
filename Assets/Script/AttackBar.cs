using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttackBar : MonoBehaviour
{
    public Slider slider;
    public new Camera camera;
    public Transform target;
    public Vector3 offset;


 public void UpdateAttackBar(float current_val, float max_val)
  {
    slider.value = current_val / max_val;
  }
   

    void Update()
    {
      transform.rotation = camera.transform.rotation;
      transform.position = target.position+offset;

        
    }
}
