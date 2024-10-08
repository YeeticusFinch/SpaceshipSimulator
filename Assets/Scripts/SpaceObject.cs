using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceObject : MonoBehaviour
{
    public string prefabPath = "";

    [NonSerialized]
    public bool isShip = false;

    public string name;

    public static int objectCount = 0;
    public int objectID = 0;

    Vector3 lerpPos = Vector3.zero;
    float lerpPosTol = 1;
    float lerpSpeed = 5;
    bool lerp = false;

    Vector3 lerpEuler = Vector3.zero;
    float lerpEulerTol = 0.5f;
    float lerpEulerSpeed = 1;
    bool lerpAngle = false;

    [NonSerialized]
    public Rigidbody rb;

    // Start is called before the first frame update
    protected void Start()
    {
        objectID = objectCount;
        objectCount++;
        rb = GetComponent<Rigidbody>();

        if (Game.instance == null)
            StartCoroutine(RecordNewObject());
        else if (Game.instance.record)
            Game.instance.rec.LogObject(this);
    }


    protected void OnCollisionEnter(Collision collision)
    {
        ShipObject o = collision.collider.GetComponentInParent<ShipObject>();
        /*SpaceShip oShip = collision.collider.GetComponentInParent<SpaceShip>();
        Missile thisM = GetComponent<Missile>();
        if (thisM == null)
            thisM = GetComponentInParent<Missile>();
        if (thisM.armed || (thisM != null && oShip != null && thisM.owner == oShip))
            return;
            */
        if (o != null)
        {
            float mass = o.rb != null ? o.rb.mass : 10;
            float relVel = collision.relativeVelocity.magnitude * (0.6f + mass / 300);
            Vector3 pos = collision.contacts[0].point;
            Debug.Log("Collision with " + o.name + " at " + relVel);
            if (relVel > 1)
            {
                Yeet.Dmg dmg = new Yeet.Dmg(0.05f * relVel, 4 * relVel, 0.01f * relVel, 0.3f * relVel, 0, 1 * relVel, 0);
                o.Damage(dmg, pos, 1);

                GameObject dmgInd = Instantiate(Game.instance.DmgIndicator) as GameObject;
                //dmgInd.tag = "Damager";
                dmgInd.transform.position = pos;
                dmgInd.transform.localScale *= dmg.GetCombinedDamage();
                dmgInd.name = "Dmger";
                dmgInd.GetComponent<Particle>().dealsDamage = true;
                dmgInd.GetComponent<Particle>().dmg = dmg;
                if (rb != null)
                    dmgInd.GetComponent<Particle>().velocity = rb.velocity;
            }
        }
    }

    protected void FixedUpdate()
    {
        if (lerp)
        {
            float dist = Vector3.Distance(lerpPos, rb.position);
            if (dist < lerpPosTol)
            {
                lerp = false;
                rb.MovePosition(lerpPos);
            } else
            {
                rb.MovePosition(rb.position + ((lerpPos - rb.position).normalized * Mathf.Min(dist, lerpSpeed)));
            }
        }
        if (lerpAngle)
        {
            float dist = Yeet.AngleDiff(lerpEuler, Yeet.FixAngle(transform.eulerAngles)).magnitude;
            if (dist < lerpEulerTol)
            {
                lerpAngle = false;
                transform.eulerAngles = lerpEuler;
            }
            else
            {
                transform.eulerAngles += (Yeet.AngleDiff(lerpEuler, Yeet.FixAngle(transform.eulerAngles).normalized * Mathf.Min(dist, lerpEulerSpeed)));
            }
        }
    }

    public void OnRecord()
    {
        if (Game.instance.record)
        {
            Game.instance.rec.LogObject(this);
        }
    }

    private void OnDestroy()
    {
        if (Game.instance.record)
        {
            Game.instance.rec.KillObject(this);
        }
    }

    IEnumerator RecordNewObject()
    {
        yield return new WaitForSeconds(0.01f);
        if (Game.instance != null && Game.instance.record)
            Game.instance.rec.LogObject(this);
    }

    public void LerpPos(Vector3 newPos)
    {
        if (rb.position == Vector3.zero)
        {
            rb.MovePosition(newPos);
        }
        else
        {
            lerpPos = newPos;
            lerp = true;
        }
    }

    public void LerpEuler(Vector3 newEuler)
    {
        if (transform.eulerAngles == Vector3.zero)
        {
            transform.eulerAngles = (newEuler);
        }
        else
        {
            lerpEuler = Yeet.FixAngle(newEuler);
            lerpAngle = true;
        }
    }
}
