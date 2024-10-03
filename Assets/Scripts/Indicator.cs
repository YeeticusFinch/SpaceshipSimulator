using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Indicator : MonoBehaviour
{

    public GameObject target;
    public Camera cam;
    public float dist;
    public bool trackObject = false;
    public bool guess = false;
    public Vector3 guessPos;
    public Vector3 guessVel;
    Canvas canvas;
    RectTransform rect;
    RectTransform canvasRect;
    Image img;
    TextMeshProUGUI txt;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
            canvas = FindObjectOfType<Canvas>();
        rect = GetComponent<RectTransform>();
        canvasRect = canvas.GetComponent<RectTransform>();
        img = GetComponent<Image>();
        txt = GetComponent<TextMeshProUGUI>();
    }

    bool visible = false;

    int c = 0;
    // Update is called once per frame
    void Update()
    {
        c++;
        c %= 10000;

        if (c % 100 == 0)
        {
            if (target != null && target.GetComponent<SpaceShip>() != null && !target.GetComponent<SpaceShip>().IsAlive())
            {
                //Debug.Log("Target is dead");
                Destroy(gameObject);
            }
        }

        if (c % 5 == 0)
        {
            if (guess)
                visible = Vector3.Angle(guessPos - cam.transform.position, cam.transform.forward) < 90;
            else if (trackObject && target != null)
                visible = Vector3.Angle(target.transform.position - cam.transform.position, cam.transform.forward) < 90;
        }

        if (guess)
        {
            if (target == null || cam == null)
            {
                //Debug.Log("Target null or cam null");
                Destroy(this.gameObject);
            }
            else
            {
                if (visible)
                {
                    if (!img.enabled)
                    {
                        img.enabled = true;
                        if (txt != null) txt.enabled = true;
                    }
                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, guessPos);
                    rect.anchoredPosition = screenPoint - canvasRect.sizeDelta / 2f;
                } else
                {
                    if (img.enabled)
                    {
                        img.enabled = false;
                        if (txt != null) txt.enabled = false;
                    }
                }

                //Vector3 fancyPos = cam.transform.InverseTransformPoint(guessPos).normalized;
                //transform.localPosition = fancyPos * dist;
                guessPos += guessVel * Time.fixedDeltaTime;
            }
        }
        else if (trackObject)
        {
            if (target == null || cam == null)
            {
                //Debug.Log("Target null or cam null");
                Destroy(this.gameObject);
            }
            else
            {
                if (visible)
                {
                    if (!img.enabled)
                    {
                        img.enabled = true;
                        if (txt != null) txt.enabled = true;
                    }
                    Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, target.transform.position);
                    rect.anchoredPosition = screenPoint - canvasRect.sizeDelta / 2f;
                }
                else
                {
                    if (img.enabled)
                    {
                        img.enabled = false;
                        if (txt != null) txt.enabled = false;
                    }
                }

                //Vector3 fancyPos = cam.transform.InverseTransformPoint(target.transform.position).normalized;
                //transform.localPosition = fancyPos * dist;



                //float r = Mathf.Sqrt(Mathf.Pow(dist, 2) + Mathf.Pow(Mathf.Sqrt(Mathf.Pow(fancyPos.x, 2) + Mathf.Pow(fancyPos.y, 2)), 2));
                //transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
