using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipObject : MonoBehaviour
{
    public bool forMissile = false;
    public float maxHP = 10;
    public float HP = 10;

    public Vector3 maxRotation;

    public Rigidbody rb;

    private Vector3 originalDir;
    
    public Yeet.Dmg dmgFactor = new Yeet.Dmg(1, 1, 1, 1, 1, 1, 1);
    public Yeet.Dmg dmgAbsorb = new Yeet.Dmg(0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f, 0.1f);

    [NonSerialized]
    public SpaceShip ship;
    [NonSerialized]
    public Reactor reactor;


    // Start is called before the first frame update
    protected void Start()
    {
        HP = maxHP;
        if (!forMissile)
        {
            if (ship == null)
                ship = GetComponentInParent<SpaceShip>();

            if (rb == null)
                rb = GetComponentInParent<Rigidbody>();
            repair();
            originalDir = transform.localEulerAngles;

            reactor = ship.reactor;
            //Debug.Log("Reactor = " + reactor);
        }
    }

    // Update is called once per frame
    protected void FixedUpdate()
    {
        if (HP > maxHP)
            HP = maxHP;
        if (HP <= 0.01f)
        {
            if (Game.instance.record && Game.instance.rec != null && ship != null && gameObject != null)
            {
                Game.instance.rec.KillChild(ship, gameObject);
            }
            if (forMissile)
                Destroy(gameObject);
            else
                gameObject.SetActive(false);
        }
    }

    public float GetHP()
    {
        return HP;
    }

    public void repair()
    {
        HP = maxHP;
    }

    public bool alive()
    {
        return HP > 0.01f;
    }

    public bool dead()
    {
        return HP <= 0.01f;
    }

    /*
    private void OnCollisionEnter(Collision collision)
    {
        float relVel = collision.relativeVelocity.magnitude;
        if (relVel > 1)
        {
            Damage(new Yeet.Dmg(0.05f * relVel, 5 * relVel, 0.01f * relVel, 1 * relVel, 0, 2 * relVel, 0), collision.contacts[0].point, 1);
        }
    }
    */

    public Yeet.Dmg Damage(Yeet.Dmg dmg, Vector3 pos, float decay = 1)
    {
        GameObject dmgInd = Instantiate(Game.instance.DmgIndicator) as GameObject;
        dmgInd.tag = "DamageIndicator";
        dmgInd.transform.position = pos;
        dmgInd.transform.localScale *= dmg.GetCombinedDamage()*2;
        dmgInd.name = "Dmg Indicator";
        if (rb != null)
            dmgInd.GetComponent<Particle>().velocity = rb.velocity;

        //dmgInd.transform.parent = transform;

        HP -= dmgFactor.bludgeoning * dmg.bludgeoning + dmgFactor.fire*dmg.fire + dmgFactor.force*dmg.force + dmgFactor.lightning*dmg.lightning + dmgFactor.piercing*dmg.piercing + dmgFactor.radiant*dmg.radiant + dmgFactor.slashing*dmg.slashing;

        dmg.bludgeoning *= 1 - dmgAbsorb.bludgeoning * decay;
        dmg.fire *= 1 - dmgAbsorb.fire * decay;
        dmg.force *= 1 - dmgAbsorb.force * decay;
        dmg.lightning *= 1 - dmgAbsorb.lightning * decay;
        dmg.piercing *= 1 - dmgAbsorb.piercing * decay;
        dmg.radiant *= 1 - dmgAbsorb.radiant * decay;
        dmg.slashing *= 1 - dmgAbsorb.slashing * decay;
        dmg.UpdateDamages();
        return dmg;
    }

    public Yeet.Dmg Damage(Yeet.Dmg dmg, float factor, Vector3 pos, float decay = 1)
    {
        GameObject dmgInd = Instantiate(Game.instance.DmgIndicator) as GameObject;
        dmgInd.tag = "DamageIndicator";
        dmgInd.transform.position = pos;
        dmgInd.transform.localScale *= dmg.GetCombinedDamage()*2;
        dmgInd.name = "Dmg Indicator";
        if (rb != null)
            dmgInd.GetComponent<Particle>().velocity = rb.velocity;
        //dmgInd.transform.parent = transform;

        float damage = dmgFactor.bludgeoning * dmg.bludgeoning + dmgFactor.fire * dmg.fire + dmgFactor.force * dmg.force + dmgFactor.lightning * dmg.lightning + dmgFactor.piercing * dmg.piercing + dmgFactor.radiant * dmg.radiant + dmgFactor.slashing * dmg.slashing;
        damage *= factor;
        HP -= damage;

        dmg.bludgeoning *= 1 - dmgAbsorb.bludgeoning * decay;
        dmg.fire *= 1 - dmgAbsorb.fire * decay;
        dmg.force *= 1 - dmgAbsorb.force * decay;
        dmg.lightning *= 1 - dmgAbsorb.lightning * decay;
        dmg.piercing *= 1 - dmgAbsorb.piercing * decay;
        dmg.radiant *= 1 - dmgAbsorb.radiant * decay;
        dmg.slashing *= 1 - dmgAbsorb.slashing * decay;
        dmg.UpdateDamages();
        return dmg;
    }
}
