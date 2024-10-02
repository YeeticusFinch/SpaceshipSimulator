using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Indicator : MonoBehaviour
{

    public GameObject target;
    public GameObject cam;
    public float dist;
    public bool trackObject = false;
    public bool guess = false;
    public Vector3 guessPos;
    public Vector3 guessVel;

    // Start is called before the first frame update
    void Start()
    {
        
    }

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

        if (guess)
        {
            if (target == null || cam == null)
            {
                //Debug.Log("Target null or cam null");
                Destroy(this.gameObject);
            }
            else
            {
                Vector3 fancyPos = cam.transform.InverseTransformPoint(guessPos).normalized;
                transform.localPosition = fancyPos * dist;
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
                Vector3 fancyPos = cam.transform.InverseTransformPoint(target.transform.position).normalized;
                transform.localPosition = fancyPos * dist;



                //float r = Mathf.Sqrt(Mathf.Pow(dist, 2) + Mathf.Pow(Mathf.Sqrt(Mathf.Pow(fancyPos.x, 2) + Mathf.Pow(fancyPos.y, 2)), 2));
                //transform.localEulerAngles = Vector3.zero;
            }
        }
    }
}
