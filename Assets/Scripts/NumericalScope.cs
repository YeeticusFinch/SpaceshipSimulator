using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NumericalScope : MonoBehaviour
{
    public Panel panel;
    public string id;
    public TextMeshProUGUI valTxt;
    float value;
    public int decimalPlaces;

    // Start is called before the first frame update
    void Start()
    {
        if (panel == null)
            panel = GetComponentInParent<Panel>();
    }

    int c = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (c % 50 == 0)
        {
            GetUpdatedValue();
        }
    }

    void GetUpdatedValue()
    {
        if (decimalPlaces == -1)
        {
            valTxt.text = panel.GetUpdatedTextScopeValue(id);
        }
        else
        {
            value = panel.GetUpdatedValue(id);
            valTxt.text = "" + Mathf.Round(value * Mathf.Pow(10, decimalPlaces)) / Mathf.Pow(10, decimalPlaces);
        }
    }
}
