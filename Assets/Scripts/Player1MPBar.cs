using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1MPBar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public void SetMaxMP()
    {
        slider.maxValue = 1f; 
        slider.value = 1f;
        fill.color = Color.blue;
    }

    public void SetMP(float MP)
    {
        slider.value = MP;
        fill.color = Color.blue;
    }
}
