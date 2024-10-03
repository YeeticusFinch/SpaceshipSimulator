using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NpcShip : MonoBehaviour
{

    public SpaceShip ship;

    public int team = 0; // 0: enemy, 1: friendly

    // 0: basic, 1: tynan, 2: rocinante
    public int style = 0;

    [NonSerialized]
    public Vector3 targetVel = Vector3.zero;

    [NonSerialized]
    public Vector3 targetPos = Vector3.zero;

    [NonSerialized]
    public SpaceShip targetShip = null;

    float relVelTol = 2;
    float matchVelTol = 5;
    float followRange = 50;
    float posTol = 10;
    float leadFac = 0.2f;
    float maxSpeedDiff = 15;
    float pdcRange = 80;
    float missileRange = 120;
    float missileFrequency = 0.9f;
    bool missileSpam = true;
    float flip_n_burn_time = 3.8f;

    void Start()
    {
        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(0.5f);
        if (ship == null)
            ship = GetComponent<SpaceShip>();
        //targetPos = ship.rb.position;
        ship.team = team;

        switch (style)
        {
            case 0: // BasicShip
                matchVelTol = 6;
                relVelTol = 3;
                followRange = 25;
                posTol = 10;
                leadFac = 0.2f;
                ship.Kp = 10;
                ship.stabalizerPower = 0.9f;
                maxSpeedDiff = 15;
                ship.pointToKd = 3;
                ship.pointToKp = 10;
                pdcRange = 80;
                missileRange = 120;
                missileFrequency = 0.1f;
                missileSpam = false;
                ship.flip_n_burn_time = 3.4f;
                ship.useInterceptMissiles = true;
                break;
            case 1: // Tynan
                matchVelTol = 6;
                relVelTol = 3;
                followRange = 55;
                posTol = 20;
                leadFac = 0.5f;
                ship.Kp = 10;
                ship.stabalizerPower = 0.9f;
                maxSpeedDiff = 15;
                ship.pointToKd = 5;
                ship.pointToKp = 8;
                pdcRange = 120;
                missileRange = 200;
                missileFrequency = 0.3f;
                missileSpam = true;
                ship.flip_n_burn_time = 4.4f;
                ship.useInterceptMissiles = true;
                break;
            case 2: // Rocinante
                matchVelTol = 6;
                relVelTol = 3;
                followRange = 35;
                posTol = 15;
                leadFac = 0.3f;
                ship.Kp = 10;
                ship.stabalizerPower = 0.9f;
                maxSpeedDiff = 15;
                ship.pointToKd = 5.584f;
                ship.pointToKp = 9.047f;
                pdcRange = 140;
                missileRange = 200;
                missileFrequency = 0.3f;
                missileSpam = true;
                ship.flip_n_burn_time = 6.2f;
                ship.useInterceptMissiles = true;
                break;
        }
        ship.useInterceptMissiles = false;
    }

    int c = 0;
    void HandleWeaponry()
    {
        if (targetShip != null)
        {
            if (true) // If missiles are enabled
            {
                if (c % 5 == 0 && Vector3.Distance(targetShip.rb.position, ship.rb.position) < missileRange && Random.Range(0, 0.999f) < missileFrequency * 0.1f)
                {
                    bool targeted = false;
                    foreach (Targeter t in ship.targeters)
                    {
                        if (t.TargetObject(targetShip.gameObject, false))
                        {
                            targeted = true;
                            break;
                        }
                    }
                    if (targeted)
                    {
                        //Debug.Log("Firing Missile");
                        foreach (MissileLauncher l in ship.launchers)
                        {
                            if (l == null)
                                continue;
                            if (l.ammo > 0)
                                l.Launch(targetShip.gameObject);
                            if (!missileSpam || Random.Range(0, 3) == 1)
                                break;
                        }
                    }
                }
            }
            if (c % 5 == 0 && Vector3.Distance(targetShip.rb.position, ship.rb.position) < pdcRange)
            {
                bool targeted = false;
                foreach (Targeter t in ship.targeters)
                {
                    if (t.TargetObject(targetShip.gameObject, false))
                    {
                        targeted = true;
                        break;
                    }
                }
                if (targeted)
                {
                    //Debug.Log("Firing PDCs");
                    foreach (gun g in ship.turrets)
                    {
                        if (g != null && g.ammo > 0 && !g.targets.ContainsKey(targetShip.gameObject))
                            g.targets.Add(targetShip.gameObject, true);
                    }
                } else
                {
                    foreach (gun g in ship.turrets)
                    {
                        if (g == null)
                            continue;
                        if (g.targets.ContainsKey(targetShip.gameObject))
                            g.targets.Remove(targetShip.gameObject);
                        if (g.targetLock == targetShip.gameObject)
                            g.targetLock = null;
                    }
                }
            }
        }
    }
    
    void FixedUpdate()
    {
        c++;
        c %= 10000;
        if (!ship.IsAlive())
            return;
        if (targetPos == Vector3.zero && ship.rb != null)
            targetPos = ship.rb.position;
        if (targetShip == null)
        {
            if (!ship.dangerObjectsPresent)
            {
                foreach (gun g in ship.turrets)
                {
                    if (g.targets.Count > 0)
                    {
                        if (g.fold)
                            g.fold = false;
                    }
                    else if (g.fold == false)
                        g.fold = true;
                }
            }
            else
            {
                foreach (gun g in ship.turrets)
                {
                    if (g.fold == true)
                        g.fold = false;
                }
            }
            if (c % 10 == 0)
            {
                SpaceShip closestEnemyShip = null;
                foreach (SpaceShip s in GameObject.FindObjectsOfType(typeof(SpaceShip)))
                {
                    if (s != null && ship.isEnemy(s))
                    {
                        if (targetShip == null || closestEnemyShip == null || Vector3.Distance(ship.rb.position, s.rb.position) < Vector3.Distance(ship.rb.position, closestEnemyShip.rb.position))
                        {
                            closestEnemyShip = s;
                            if (ship.CanSee(s))
                                targetShip = s;
                        }
                    }
                }

                //Debug.Log("Closest Ship = " + closestEnemyShip);

                if (targetShip == null && closestEnemyShip != null && (targetPos.magnitude == 0 || Vector3.Distance(targetPos, ship.rb.position) < 5 || Random.Range(0, 100) > 98))
                {
                    if (Vector3.Distance(ship.rb.position, closestEnemyShip.rb.position) > followRange * 5)
                    {
                        targetPos = closestEnemyShip.rb.position + 5 * followRange * (new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)));
                    }
                }
            }
        }
        else
        {
            if (!targetShip.IsAlive())
            {
                targetShip = null;
            }
            else
            {
                foreach (gun g in ship.turrets)
                {
                    if (g.fold == true)
                        g.fold = false;
                }
                HandleWeaponry();
                if (Vector3.Magnitude(-targetShip.rb.velocity + ship.rb.velocity) > matchVelTol)
                {
                    targetVel = targetShip.rb.velocity;
                    //Debug.Log("targetVel -> " + targetShip.rb.velocity);
                }
                if (Vector3.Distance(targetShip.rb.position, ship.rb.position) > followRange)
                {
                    targetPos = targetShip.rb.position + targetShip.rb.velocity * Time.fixedDeltaTime * Vector3.Distance(targetShip.rb.position, ship.rb.position) * leadFac;
                    //Debug.Log("targetPos -> " + targetShip.rb.position);
                }
            }
        }
        ship.velRef = targetVel;

        //Debug.Log("TargetShip = " + targetShip);
        //Debug.Log("Pos = " + ship.rb.position + ", TargetPos = " + targetPos);
        //String str = "";
        //if (Vector3.Magnitude(ship.velRef - ship.rb.velocity) > relVelTol)
        /*if (Vector3.Magnitude(Yeet.Perp(ship.rb.velocity, targetVel)) > relVelTol || Vector3.Magnitude(ship.velRef - ship.rb.velocity) > maxSpeedDiff)
        {
            ship.flip_n_burn = true;
            ship.goToPoint = Vector3.zero;
            str += "Matching course: ";
            str += "[ship:" + ship.rb.velocity + ", target:" + targetVel + "] ";
            if (Vector3.Magnitude(Yeet.Perp(ship.rb.velocity, targetVel)) > relVelTol)
                str += " perp, ";
            else
                str += " mag, ";
        }
        else
        {*/
            //str += "[ship:" + ship.rb.velocity + ", target:" + targetVel + "] ";
            ship.flip_n_burn = false;
        if (targetPos != Vector3.zero)
        {
            if (ship != null && ship.rb != null && Vector3.Distance(targetPos, ship.rb.position) > posTol)
            {
                ship.goToPoint = targetPos;
                ship.flip_n_burn = false;
                //str += "Getting closer, ";
            }
            else
            {
                targetPos = Vector3.zero;
                ship.goToPoint = Vector3.zero;
                //str += "Stopped getting closer, ";
                if (Vector3.Magnitude(ship.velRef - ship.rb.velocity) > relVelTol)
                {
                    ship.flip_n_burn = true;
                    ship.goToPoint = Vector3.zero;
                    //str += "[Matching course]";
                }
            }
        }
        //}
        //Debug.Log(str);
    }
}
