using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FancySlider : MonoBehaviour
{
    public Panel panel;
    public string id;
    [NonSerialized]
    public float value;

    public Slider slider;

    public float min = 0;
    public float max = 1;

    public TextMeshProUGUI valueTxt;

    bool initted = false;

    // Start is called before the first frame update
    void Start()
    {
        if (panel == null)
            panel = GetComponentInParent<Panel>();
    }

    int c = 0;
    void FixedUpdate()
    {
        c++;
        c %= 1000;
        if (initted && c % 10 == 0 && value != toVal(slider.value))
        {
            value = toVal(slider.value);
            panel.PanelSlider(id, value);
            valueTxt.text = "" + Mathf.Round(value * 1000) / 1000f;
        }
        if (c % 50 == 0)
        {
            GetUpdatedSliderValue();
        }
    }

    void GetUpdatedSliderValue()
    {
        initted = true;
        value = panel.GetUpdatedSlider(id);
        slider.value = toDec(value);
        valueTxt.text = "" + Mathf.Round(value * 1000) / 1000f;
    }

    float toVal(float x)
    {
        return x * (max - min) + min;
    }
    
    float toDec(float y)
    {
        return (y - min) / (max - min);
    }
}
