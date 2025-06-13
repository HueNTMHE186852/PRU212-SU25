using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1MPBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    [SerializeField] private Gradient gradient;
    public void SetMaxMP()
    {
        slider.maxValue = 1f; 
        slider.value = 1f;   
        fill.color = gradient.Evaluate(1f);
    }

    public void SetMP(float MP)
    {
        slider.value = MP;
        Debug.Log(slider.value);
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
