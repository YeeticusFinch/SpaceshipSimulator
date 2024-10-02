using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reactor : ShipObject
{
    public bool power = false;
    public bool drivePower = false;
    public bool weaponPower = false;
    public bool commsPower = false;
    public bool telemetryPower = false;
    public float electricNoise = 20;
    public bool batteryPower = false;

    [System.NonSerialized]
    public int coreSize = 10;

    [System.NonSerialized]
    public bool lowPower = false;
    [System.NonSerialized]
    public bool highPower = false;
    [System.NonSerialized]
    public float reactorStability = 100;

    [System.NonSerialized]
    public float batteryAmount = 0;

    public int batteryCapacity = 100;

    public float chargeRate = 2f;

    [System.NonSerialized]
    public bool tryingToPower = false;

    void Start()
    {
        base.Start();
        if (ship.trophy)
            return;
        batteryAmount = batteryCapacity;
        HP = maxHP;
        PowerUp();
        reactorStability = 100;
    }

    int c = 0;
    void FixedUpdate()
    {
        if (ship.trophy)
            return;
        base.FixedUpdate();
        c++;
        c %= 10000;
        if (coreSize < 6 || coreSize > 14)
            power = false;
        else if (tryingToPower)
            power = true;
        if (c % 10 == 0 && power)
        {
            if (reactorStability < 90)
            {
                coreSize += (int)(Random.Range(-1, 1) * (90 - reactorStability) * 0.1f);
            }
            if (batteryAmount < batteryCapacity)
                batteryAmount += chargeRate;
            else if (batteryAmount > batteryCapacity)
                batteryAmount = batteryCapacity;
            if (HP/maxHP < 0.8f)
            {
                reactorStability -= (lowPower ? 0.4f : 1) * 0.1f * (0.8f - HP / maxHP);
            }
            if (highPower)
                reactorStability -= 0.1f;
            if (reactorStability <= 0.01f)
                Explode();
        }
        if (HP <= 0 || (power == false && batteryPower == false))
        {
            power = false;
            batteryPower = false;
            drivePower = false;
            weaponPower = false;
            commsPower = false;
            telemetryPower = false;
            batteryAmount = 0;
        }
        if (batteryAmount <= 0.001f && batteryPower) batteryPower = false;
    }

    public void DropCore()
    {
        tryingToPower = false;
        power = false;
        StartCoroutine(GrowCore());
    }

    public void PowerUp()
    {
        if (HP > 0)
        {
            tryingToPower = true;
            power = true;
            batteryPower = true;
            drivePower = true;
            weaponPower = true;
            commsPower = true;
            telemetryPower = true;
        }
    }

    public void PowerDown()
    {
        tryingToPower = false;
        batteryPower = false;
        power = false;
        drivePower = false;
        weaponPower = false;
        commsPower = false;
        telemetryPower = false;
    }

    public void DamageCore(float amount)
    {
        if (power)
        {
            reactorStability -= (highPower ? 2f : (lowPower ? 0.5f : 1)) * 0.3f * amount;
        }
    }
    bool exploded = false;

    public void Explode()
    {
        if (!exploded)
        {
            exploded = true;
            StartCoroutine(ExplodeCore());
        }
    }

    IEnumerator ExplodeCore()
    {
        yield return new WaitForSeconds(0.1f);
        GameObject exp = Instantiate(Resources.Load("Explosions/Explosion")) as GameObject;
        exp.transform.position = transform.position;
        exp.GetComponent<Explosion>().vel = rb.GetPointVelocity(transform.position);
        exp.GetComponent<Explosion>().radius = 10;
        exp.GetComponent<Explosion>().density = 10;
        exp.GetComponent<Explosion>().dmg.ScaleDamageAndOG(15f);
        exp.GetComponent<Explosion>().delay = 0.1f;
        //yield return new WaitForSeconds(0.1f);
        GameObject exp2 = Instantiate(Resources.Load("Explosions/Explosion")) as GameObject;
        exp2.transform.position = transform.position;
        exp2.GetComponent<Explosion>().vel = rb.GetPointVelocity(transform.position);
        exp2.GetComponent<Explosion>().radius = 20;
        exp2.GetComponent<Explosion>().density = 10;
        exp2.GetComponent<Explosion>().dmg.ScaleDamageAndOG(10f);
        exp2.GetComponent<Explosion>().delay = 0.2f;
        //yield return new WaitForSeconds(0.1f);
        GameObject exp3 = Instantiate(Resources.Load("Explosions/Explosion")) as GameObject;
        exp3.transform.position = transform.position;
        exp3.GetComponent<Explosion>().vel = rb.GetPointVelocity(transform.position);
        exp3.GetComponent<Explosion>().radius = 30;
        exp3.GetComponent<Explosion>().density = 5;
        exp3.GetComponent<Explosion>().dmg.ScaleDamageAndOG(6f);
        exp3.GetComponent<Explosion>().delay = 0.3f;
        //yield return new WaitForSeconds(0.1f);
        GameObject exp4 = Instantiate(Resources.Load("Explosions/Explosion")) as GameObject;
        exp4.transform.position = transform.position;
        exp4.GetComponent<Explosion>().vel = rb.GetPointVelocity(transform.position);
        exp4.GetComponent<Explosion>().radius = 40;
        exp4.GetComponent<Explosion>().density = 5;
        exp3.GetComponent<Explosion>().dmg.ScaleDamageAndOG(4f);
        exp4.GetComponent<Explosion>().delay = 0.4f;
        //exp.GetComponent<Explosion>()
        HP = 0;
        reactorStability = 0;
        PowerDown();
    }

    IEnumerator GrowCore()
    {
        coreSize = 0;
        for (int i = 0; i < 10; i++)
        {
            tryingToPower = false;
            power = false;
            yield return new WaitForSeconds(1);
            coreSize++;
        }
        PowerUp();
    }
}
