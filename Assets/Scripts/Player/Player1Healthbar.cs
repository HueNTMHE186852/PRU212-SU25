using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1Healthbar : MonoBehaviour
{
    public Slider slider;
    public Image fill;
    public void SetMaxHealth()
    {
        slider.maxValue = 1f;
        slider.value = 1f;
        fill.color = Color.red;
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = Color.red;
    }
}
