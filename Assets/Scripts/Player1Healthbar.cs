using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1Healthbar : MonoBehaviour
{
    public Slider slider;

    [SerializeField] private Gradient gradient;
    public Image fill;
    public void SetMaxHealth()
    {
        slider.maxValue = 1f;
        slider.value = 1f;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
