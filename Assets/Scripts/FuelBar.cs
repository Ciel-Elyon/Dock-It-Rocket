using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelBar : MonoBehaviour
{
    private Slider slider;
    public Gradient gradient;
    public Image fill;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }

    public void SetFuelAmount(float amt)
    {
        slider.value = amt;

        fill.color = gradient.Evaluate(slider.normalizedValue);
    }

    public void SetMaxFuel(float amt)
    {
        slider.maxValue = amt;
        slider.value = amt;

        fill.color = gradient.Evaluate(1f);
    }
}
