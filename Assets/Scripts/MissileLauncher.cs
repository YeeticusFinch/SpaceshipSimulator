using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileLauncher : ShipObject
{
    public bool foldable = false;
    [NonSerialized]
    public bool folded = false;
    [NonSerialized]
    public bool fold = false;
    public int launcherType = 0;
    public GameObject[] movingParts;

    public Missile missile;
    public int maxAmmo;
    public int launchDelay = 20;
    public Vector3 spawnPoint;
    public Vector3 safePos;
    public Vector3 safeDir = new Vector3(0, 1, 0);
    public Vector3 launchVel = new Vector3(0, 0, 0);
    public bool skipSafePos = false;
    public float electricNoise = 0.05f;
    public float batteryConsumption = 0.1f;
    public SoundManager.Sound foldSound;
    public SoundManager.Sound unfoldSound;

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
    int maxFoldIndex = 1;
    int foldIndex = -1;
    bool playSounds = false;

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
        if (launcherType == 0 || launcherType == 1 || launcherType == 2 || launcherType == 5)
            maxFoldIndex = 2;
        if (launcherType == 3 || launcherType == 4)
            maxFoldIndex = 4;
        if (foldable)
            fold = true;
        playSounds = ship.playSounds;
    }

    int c = 0;
    void FixedUpdate()
    {
        base.FixedUpdate();
        c++;
        c %= 10000;
        if (!playSounds && c % 10 == 0 && ship.playSounds) playSounds = true;
        if (reactor == null)
            return;
        if ((folded != fold) && foldable)
        {
            if (!reactor.power)
                reactor.batteryAmount -= batteryConsumption;
            if (fold)
                FoldSequence(1);
            else
                FoldSequence(-1);
        }
        if ((folded || fold) && foldable)
            return;
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
        if (foldable && (folded || fold))
            return;
        if (target == null)
            return;
        this.target = target;
        launch = true;
        SpaceShip targetShip = target.GetComponent<SpaceShip>();
        if (targetShip != null)
        {
            if (targetShip.playerShip)
                targetShip.player.addAlert("Incoming missile!", Color.red);
        }
    }

    void FoldSequence(int dir)
    {
        if (!foldable || movingParts == null) return;
        if (foldIndex == -1)
            foldIndex = folded ? maxFoldIndex : 0;

        if (launcherType == 1)
        {
            if (movingParts.Length < 3) return;
            bool c = true;
            switch (foldIndex)
            {
                case 0: // Unfolded Position
                    //c = TranslateStep(movingParts[0], new Vector3(0, 0, -0.001f), 0.01f) && c;
                    //c = TranslateStep(movingParts[1], new Vector3(0, -0.0108f, -0.0128f), 0.01f) && c;
                    //c = TranslateStep(movingParts[2], new Vector3(0, 0.0108f, -0.0128f), 0.01f) && c;
                    c = RotateStep(movingParts[0], new Vector3(90, 0, -180), 5) && c;
                    //c = c && RotateStep(movingParts[2], new Vector3(-90, 0, 0), 5);
                    //c = RotateStep(movingParts[2], new Vector3(270, 0, 0), 5) && c;
                    if (c)
                    {
                        folded = false;
                        if (dir > 0)
                        {
                            foldIndex++;
                            if (playSounds)
                                unfoldSound.play(transform.position);
                        }
                    }
                    break;
                case 1: // Close Doors
                    //c = TranslateStep(movingParts[0], new Vector3(0, 0, -0.0157f), 0.01f) && c;
                    //c = TranslateStep(movingParts[1], new Vector3(0, -0.0108f, -0.0003632093f), 0.01f) && c;
                    //c = TranslateStep(movingParts[2], new Vector3(0, 0.0108f, -0.0003632093f), 0.01f) && c;
                    c = RotateStep(movingParts[0], new Vector3(0, 2.612f, -181.718f), 5) && c;
                    //c = RotateStep(movingParts[2], new Vector3(359, 0, 0), 5) && c;
                    if (c)
                    {
                        folded = true;
                        if (dir < 0)
                        {
                            foldIndex--;
                            if (playSounds)
                                foldSound.play(transform.position);
                        }
                    }
                    break;
            }
        }
    }

    bool RotateStep(GameObject obj, Vector3 rot, float speed, bool flip_y_z = false, bool funnyDiff = false)
    {
        if (obj == null)
            return true;
        if (flip_y_z)
            rot = new Vector3(rot.x, rot.z, rot.y);
        float dist = funnyDiff ? AngleDiff(FixAngle(obj.transform.localEulerAngles), FixAngle(rot)).magnitude : Vector3.Distance(obj.transform.localEulerAngles, rot);
        if (dist < 0.5f)
        {
            obj.transform.localEulerAngles = rot;
            return true;
        }
        else
        {
            obj.transform.localEulerAngles += (rot - obj.transform.localEulerAngles).normalized * speed;
            return false;
        }
    }

    bool TranslateStep(GameObject obj, Vector3 pos, float speed, bool local = false)
    {
        if (obj == null)
            return true;
        Vector3 currentPos = local ? obj.transform.localPosition : obj.transform.position;
        float dist = Vector3.Distance(currentPos, pos);
        if (dist < 0.001f)
        {
            if (local) obj.transform.localPosition = pos;
            else obj.transform.position = pos;
            return true;
        }
        else
        {
            Vector3 nextPos = currentPos + (pos - currentPos).normalized * speed;
            if (local) obj.transform.localPosition = nextPos;
            else obj.transform.position = nextPos;
            return false;
        }
    }

    Vector3 FixAngle(Vector3 rot)
    {
        return new Vector3(rot.x % 360, rot.y % 360, rot.z % 360);
    }

    Vector3 AngleDiff(Vector3 a, Vector3 b)
    {
        Vector3 result = b - a;
        if (result.x > 180) result -= 360 * Vector3.right;
        if (result.x < -180) result += 360 * Vector3.right;
        if (result.y > 180) result -= 360 * Vector3.up;
        if (result.y < -180) result += 360 * Vector3.up;
        if (result.z > 180) result -= 360 * Vector3.forward;
        if (result.z < -180) result += 360 * Vector3.forward;
        return result;
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
        m.setVelocity(ship.rb.velocity + transform.TransformDirection(launchVel));
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
