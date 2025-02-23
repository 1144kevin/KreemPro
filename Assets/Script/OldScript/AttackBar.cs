using UnityEngine;
using UnityEngine.UI;


public class AttackBar : MonoBehaviour
{
    public Slider slider;
    public Gradient gradient;
    public Image fill;

    public void SetMaxAttack(float attack){
        slider.maxValue = attack;
        slider.value=attack;

        fill.color=gradient.Evaluate(1f);
    }

    public void SetAttack(float attack){
        slider.value = attack;
        fill.color=gradient.Evaluate(slider.normalizedValue);
    }
 
}


