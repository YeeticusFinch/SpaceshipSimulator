using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FancyList : MonoBehaviour
{
    public Panel panel;
    public string id;
    //[System.NonSerialized]
    //public TextMeshProUGUI valueTxt;
    public string[] values;
    public int maxLines = 20;
    public string topText = "Components:";
    public float spacing = 2;
    public float width = 260;
    public int fontSize = 24;
    public int offset = 0;
    List<TextMeshProUGUI> valueTxts;

    // Start is called before the first frame update
    void Start()
    {

        valueTxts = new List<TextMeshProUGUI>(maxLines+1);

        for (int i = 0; i < maxLines+1; i++)
        {
            GameObject temp = new GameObject("List Object");
            temp.transform.parent = transform;
            temp.transform.position = transform.position + Vector3.down * i * spacing;
            valueTxts.Add(temp.AddComponent<TextMeshProUGUI>());
            valueTxts[i].fontSize = fontSize;
            temp.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
        }

        valueTxts[0].text = topText;

        //valueTxt = GetComponent<TextMeshProUGUI>();
    }

    int c = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        c++;
        c %= 10000;
        if (c % 15 == 0)
        {
            GetUpdatedValues();
        }
    }

    public void GetUpdatedValues()
    {
        values = panel.GetUpdatedListValue(id);
        offset %= values.Length;
        if (offset % maxLines != 0)
            offset -= offset % maxLines;
        for (int i = 0; i < maxLines; i++)
        {
            if (i + offset >= values.Length)
            {
                valueTxts[i + 1].text = "---";
                continue;
            }
            if (values[i + offset].Contains("%r"))
            {
                valueTxts[i + 1].color = Color.red;
            }
            else if (values[i + offset].Contains("%y"))
            {
                valueTxts[i + 1 + offset].color = Color.yellow;
            }
            else
            {
                valueTxts[i + 1].color = Color.white;
            }
            valueTxts[i + 1].text = values[i+offset].Replace("%r", "").Replace("%y", "");
        }
    }
}
