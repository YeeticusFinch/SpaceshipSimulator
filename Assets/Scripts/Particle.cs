using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Particle : MonoBehaviour
{

    public int lifetime = 20;
    public bool shrinking = true;

    public float randSize = 0.1f;
    public float randSpeed = 0.1f;
    public int randLifeTime = 1;
    public bool paused = false;

    [NonSerialized]
    public Vector3 velocity;
    [NonSerialized]
    public Vector3 localVelocity;

    public int grace = 3;

    Vector3 ogVelocity;

    public Collider col;
    public Collider missileCol;

    [NonSerialized]
    public bool forMissile = false;

    [NonSerialized]
    public float c = 0;

    public Yeet.Dmg dmg = new Yeet.Dmg(0, 0, 0, 0, 0, 0, 0);
    public Vector3 hitscanDir = Vector3.up;
    public bool dealsDamage = false;
    public bool noFunnyColliders = false;
    
    public bool hitscan = false;
    public float hitscanThickness = 0.1f;
    public HitscanBlast[] hitscanBlasts;

    public float decay = 1;
    
    [Serializable]
    public struct HitscanBlast
    {
        public HitscanBlast(int offset, int range, float delay, float dmgPercent, float resolution, bool lr = false, int segFreq = 1, float segRand = 0, Material segMat = null, int lrLife = 5, float lrWidth = 0.04f)
        {
            this.offset = offset;
            this.range = range;
            this.delay = delay;
            this.dmgPercent = dmgPercent;
            this.resolution = resolution;
            rangeMult = 1;
            this.lr = lr;
            this.segFreq = segFreq;
            this.segRand = segRand;
            this.segMat = segMat;
            this.lrLife = lrLife;
            this.lrWidth = lrWidth;
        }
        public void SetRangeMult(float rangeMult)
        {
            this.rangeMult = rangeMult;
        }
        public int offset;
        public int range;
        public float delay;
        public float dmgPercent;
        public float resolution;
        public float rangeMult;
        public bool lr;
        public int segFreq;
        public float segRand;
        public Material segMat;
        public int lrLife;
        public float lrWidth;
    }

    List<Collider> HitscanShoot(HitscanBlast hb, List<Collider> colliders = null)
    {
        List<Collider> result = colliders == null ? new List<Collider>() : colliders;
        float range = hb.range - hb.offset;
        Vector3 pos = transform.position + transform.TransformDirection(hitscanDir) * hb.offset;
        Vector3 hitPos = transform.position;

        while (range > 0)
        {
            RaycastHit hit;
            if (Physics.SphereCast(pos, hitscanThickness, transform.TransformDirection(hitscanDir), out hit, range, Game.RadarMask))
            {
                range -= Vector3.Distance(pos, hit.point);
                pos = hit.point + hb.resolution * transform.TransformDirection(hitscanDir);
                if (!result.Contains(hit.collider))
                {
                    result.Add(hit.collider);
                    float yeet = TriggerEnter(hit.collider, 2, pos);
                    range *= yeet;
                    hb.SetRangeMult(hb.rangeMult * yeet);
                    hitPos = hit.point;
                }
            }
            else
            {
                break;
            }
        }

        if (hb.lr)
        {
            GameObject temp = GameObject.Instantiate(Resources.Load("Particles/Bullets/Beam")) as GameObject;
            Particle tempP = temp.GetComponent<Particle>();
            tempP.lifetime = hb.lrLife;
            temp.transform.position = transform.position;
            LineRenderer lr = temp.GetComponent<LineRenderer>();
            lr.startWidth = hb.lrWidth;
            lr.endWidth = hb.lrWidth;
            List<Material> mats = new List<Material>();
            mats.Add(hb.segMat);
            lr.SetMaterials(mats);
            lr.positionCount = hb.segFreq + 1;
            lr.SetPosition(0, transform.position);
            float finalRange = Mathf.Max(range, Vector3.Distance(transform.position, hitPos));
            Vector3 finalPoint = pos + transform.TransformDirection(hitscanDir) * finalRange;
            if (hb.segFreq <= 1)
            {
                lr.SetPosition(1, finalPoint);
            } else
            {
                for (int i = 1; i < hb.segFreq + 1; i++)
                {
                    //float yoink = (i+1) / (hb.segFreq);
                    Vector3 rand = (i == hb.segFreq - 1) ? Vector3.zero : hb.segRand * (new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
                    Vector3 yeetPos = ((hb.segFreq - (float)i)/hb.segFreq) * transform.position + (((float)i)/hb.segFreq) * finalPoint + rand;
                    lr.SetPosition(i, yeetPos);
                }
            }
        }

        return result;
    }

    public IEnumerator HitscanFireRoutine()
    {
        List<Collider> colliders = new List<Collider>();
        float rangeMult = 1;
        foreach (HitscanBlast hb in hitscanBlasts)
        {
            hb.SetRangeMult(rangeMult);
            yield return new WaitForSeconds(hb.delay);
            colliders.AddRange(HitscanShoot(hb, colliders));
            rangeMult = hb.rangeMult;
            if (rangeMult < 0.001f)
                break;
        }
        yield return new WaitForSeconds(0.5f);
        if (this != null && this.gameObject != null)
            GameObject.Destroy(this.gameObject);
    }

    public SoundManager.Sound impactSound;
    
    // Start is called before the first frame update
    void Start()
    {
        if (noFunnyColliders)
        {
            FunnyCollision funny = gameObject.AddComponent<FunnyCollision>();
            funny.particle = this;
            funny.code = 0;
        }
        lifetime += Random.Range(-randLifeTime, randLifeTime);
        lifetime = Mathf.Max(1, lifetime);
        transform.localScale *= 1 + Random.Range(-randSize, randSize);
        velocity *= 1 + Random.Range(-randSpeed, randSpeed);

        //col = GetComponent<Collider>();
    }

    string recap = "";

    // Update is called once per frame
    void FixedUpdate()
    {
        if (paused) return;
        c += 1;
        if (c == 3)
            ogVelocity = velocity;
        if (c == grace && col != null)
            col.enabled = true;
        if (c == grace && forMissile && missileCol != null)
            missileCol.enabled = true;
        if (shrinking)
            transform.localScale *= 1-(c/(lifetime));
        if (c > lifetime && !hitscan)
        {
            GameObject.Destroy(this.gameObject);
        }
        if (Game.instance != null && Game.instance.playbackRecording)
        {
            transform.position += velocity * Time.fixedDeltaTime * Game.instance.rec.playbackSpeed;
            transform.localPosition += localVelocity * Time.fixedDeltaTime * Game.instance.rec.playbackSpeed;
        } else
        {
            transform.position += velocity * Time.fixedDeltaTime;
            transform.localPosition += localVelocity * Time.fixedDeltaTime;
        }
    }

    public float TriggerEnter(Collider other, int code, Vector3 point = new Vector3()) // code 0 is the regular collider, code 1 is the collider for missiles
    {
        if (point.magnitude < 0.0001f) point = transform.position;
        GameObject obj = other.gameObject;
        bool hitMissile = obj.GetComponentInParent<Missile>() != null || (obj.GetComponentInParent<SimpleMotionObject>() && obj.GetComponentInParent<SimpleMotionObject>().useMissileCollider) || obj.tag == "ShootTarget";
        if (!hitMissile && code == 1) return 1;

        if (obj.tag == "Explosion" || obj.tag == "DamageIndicator" || obj.tag == "Bullet") return 1;
        if (obj.tag == "BigThrust")
        {
            if (code != 2)
                GameObject.Destroy(this.gameObject);
            else
                return 0;
        }

        //GameObject.Destroy(this.gameObject);
        
        ShipObject shipObj = obj.GetComponent<ShipObject>();
        int i = 0;
        while (shipObj == null && obj.transform.parent != null && i < 10)
        {
            obj = obj.transform.parent.gameObject;
            shipObj = obj.GetComponent<ShipObject>();
            i++;
        }
        if (shipObj != null)
        {
            if (dealsDamage)
            {
                //recap += dmg.GetCombinedDamage() + " dmg to " + shipObj.name + ", ";
                dmg = shipObj.Damage(dmg, other.ClosestPoint(point), decay);
                //recap += "HP=" + Mathf.Round(shipObj.GetHP()) + "; ";
                if (dmg.GetCombinedDamage() < 0.001f)
                {
                    if (code != 2)
                        GameObject.Destroy(this.gameObject);
                    else
                        return 0;
                }
                velocity = ogVelocity * (Mathf.Min(Mathf.Max(dmg.GetDamagePercent(), 0.3f), 1));
            }
            if ((shipObj.GetComponentInParent<SpaceShip>() != null && shipObj.GetComponentInParent<SpaceShip>().playSounds) || (Game.instance != null && Game.instance.atmosphere))
                impactSound.play(point);
            return (Mathf.Min(Mathf.Max(dmg.GetDamagePercent(), 0.3f), 1));
        }
        else if (obj.tag == "AlwaysTarget")
        {
            GameObject dmgInd = Instantiate(Game.instance.TargetDmgIndicator) as GameObject;
            dmgInd.tag = "DamageIndicator";
            dmgInd.transform.position = point;
            dmgInd.transform.localScale *= dmg.GetCombinedDamage() * 2;
            dmgInd.name = "Target Dmg Indicator";
            //if (obj.GetComponent<Rigidbody>() != null)
            //    dmgInd.GetComponent<Particle>().velocity = obj.GetComponent<Rigidbody>().velocity;
            if (code != 2)
                GameObject.Destroy(this.gameObject);
            else
                return 0;
        } else if (obj.tag == "ShootTarget")
        {
            GameObject dmgInd = Instantiate(Game.instance.TargetDmgIndicator) as GameObject;
            dmgInd.tag = "DamageIndicator";
            dmgInd.transform.position = point;
            dmgInd.transform.localScale *= dmg.GetCombinedDamage() * 30;
            dmgInd.name = "Target Dmg Indicator";

            if (obj.GetComponent<Goal>() != null)
                obj.GetComponent<Goal>().HP -= dmg.GetCombinedDamage();
            //if (obj.GetComponent<Rigidbody>() != null)
            //    dmgInd.GetComponent<Particle>().velocity = obj.GetComponent<Rigidbody>().velocity;
            if (code != 2)
                GameObject.Destroy(this.gameObject);
            else
                return 0;
        }
        else
        {
            /*GameObject dmgInd = Instantiate(Game.instance.DmgIndicator) as GameObject;
            dmgInd.tag = "DamageIndicator";
            dmgInd.transform.position = point;
            dmgInd.transform.localScale *= dmg.GetCombinedDamage() * 2;
            dmgInd.name = "Dmg Indicator";
            //if (obj.GetComponent<Rigidbody>() != null)
            //    dmgInd.GetComponent<Particle>().velocity = obj.GetComponent<Rigidbody>().velocity;
            GameObject.Destroy(this.gameObject);
            */
        }
        return 1;
    }

    private void OnDestroy()
    {
        //Debug.Log(recap);
    }

    public void TriggerExit(Collider other, int code)
    {

    }

    public void OnCollisionEnter(Collision collision)
    {
        GameObject.Destroy(this.gameObject);
    }
}
