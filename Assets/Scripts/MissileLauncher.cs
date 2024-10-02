using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : ShipObject
{
    public Missile missile;
    public int maxAmmo;
    public int launchDelay = 20;
    public Vector3 spawnPoint;
    public Vector3 safePos;
    public Vector3 safeDir = new Vector3(0, 1, 0);
    public Vector3 launchVel = new Vector3(0, 0, 0);
    public bool skipSafePos = false;
    public float electricNoise = 0.05f;

    [NonSerialized]
    public int ammo;

    [NonSerialized]
    public bool defend = false;

    [NonSerialized]
    Indicator targetIndi;

    [NonSerialized]
    GameObject target;

    [NonSerialized]
    bool launch = false;

    public bool intercept = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (safeDir.magnitude < 0.1f)
            safeDir = new Vector3(0, 1, 0);
        ammo = maxAmmo;
        if (ship == null)
            ship = GetComponentInParent<SpaceShip>();
        if (reactor == null)
            reactor = ship.reactor;
    }

    int c = 0;
    void FixedUpdate()
    {
        base.FixedUpdate();
        c++;
        c %= 10000;
        if (c % launchDelay == 0 && launch && ammo > 0)
        {
            if (!Game.instance.playbackRecording)
                FireMissile();
            launch = false;
            target = null;
            ammo--;
        }
    }

    public void Launch(GameObject target)
    {
        this.target = target;
        launch = true;
    }

    public void Launch(GameObject target, Indicator targetIndi)
    {
        Launch(target);
        this.targetIndi = targetIndi;
    }

    void FireMissile()
    {
        GameObject o = Instantiate(missile.gameObject, transform.TransformPoint(spawnPoint), transform.rotation) as GameObject;
        Missile m = o.GetComponent<Missile>();
        o.GetComponent<Rigidbody>().velocity = ship.rb.velocity + transform.TransformDirection(launchVel);
        m.safePos = transform.TransformPoint(safePos);
        m.safeVel = ship.rb.velocity + transform.TransformDirection(launchVel);
        m.target = target;
        m.armed = false;
        m.teakettle = true;
        m.owner = ship;
        m.targetIndi = targetIndi;
        m.safeDir = transform.TransformDirection(safeDir);
        if (skipSafePos)
        {
            m.armed = true;
            m.matchCourse = false;
            m.teakettle = false;
        }
        if (ship.playerShip && ship.player.missileCam)
        {
            ship.player.missileCam = m;
            m.useCamera = true;
            m.cam = ship.player.cam.gameObject;
            m.ogCamSpot = ship.player.cam.gameObject.transform.parent.gameObject;
            m.cam.transform.parent = m.camSpot.transform;
            m.cam.transform.localPosition = Vector3.zero;
            m.cam.transform.localEulerAngles = Vector3.zero;
            if(ship.player.missileControl)
            {
                m.manualControl = true;
            }
        }

        if (Game.instance.record)
        {
            int launcherIndex = -1;
            for (int i = 0; i < ship.launchers.Length && launcherIndex == -1; i++)
                if (ship.launchers[i] == this)
                    launcherIndex = i;
            Game.instance.rec.LogMissileLaunch(ship, m, launcherIndex);
        }
    }
    
}
