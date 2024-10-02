using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Events;

public class FunnyCollision : MonoBehaviour
{
    public Particle particle;
    public int code = 0;
    public bool continuous = true;
    public bool simplify = false;
    public float radius = 0.1f;
    //public UnityEvent onTriggerEnter;

    //public UnityEvent onTriggerExit;

    // Start is called before the first frame update
    void Start()
    {

    }

    Vector3 prevPos;
    // Update is called once per frame
    void FixedUpdate()
    {
        if (continuous && particle != null && particle.c >= particle.grace)
        {
            if (prevPos.magnitude > 0.0001f)
            {
                RaycastHit hit;
                if (simplify)
                {
                    if (Physics.Linecast(transform.position, prevPos, out hit, Game.RadarMask))
                    {
                        particle.TriggerEnter(hit.collider, code);
                    }
                }
                else
                {
                    if (Physics.SphereCast(prevPos, radius, transform.position - prevPos, out hit, Vector3.Distance(transform.position, prevPos), Game.RadarMask))
                    {
                        particle.TriggerEnter(hit.collider, code);
                    }
                }
            }
            prevPos = transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (particle != null)
            particle.TriggerEnter(other, code);
    }

    private void OnTriggerExit(Collider other)
    {
        //if (code == 1) return;
        if (particle != null)
            particle.TriggerExit(other, code);
    }
}
