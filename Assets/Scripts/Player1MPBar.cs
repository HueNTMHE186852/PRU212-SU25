using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player1MPBar : MonoBehaviour
{
    public Slider slider;

    [SerializeField] private Gradient gradient;
    public Image fill;
    public void SetMaxMP(int MP)
    {
        slider.maxValue = MP;
        slider.value = MP;
        fill.color = gradient.Evaluate(1f);
    }

    public void SetMP(float MP)
    {
        slider.value = MP;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
