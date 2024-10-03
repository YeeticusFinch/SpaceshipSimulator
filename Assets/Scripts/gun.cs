using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gun : ShipObject
{
    public bool foldable = false;
    public float electricNoise = 2;
    public float shootNoise = 0.5f;
    public float batteryConsumption = 0.1f;

    [NonSerialized]
    public bool folded = false;
    [NonSerialized]
    public bool fold = false;

    public bool turret = false;
    public bool stationary = false;

    [NonSerialized]
    public GameObject targetLock;
    [NonSerialized]
    public Dictionary<GameObject, bool> targets = new Dictionary<GameObject, bool>();
    private List<GameObject> targetsToRemove = new List<GameObject>();

    public bool hitscan = false;
    public GameObject bullet;
    public GameObject bullet2;
    public GameObject bullet3;
    public float bulletThickness = 0.035f;
    public GameObject muzzleFlash;
    public GameObject muzzleFlash2;
    public GameObject muzzleFlash3;
    public GameObject cam;
    public Yeet.Dmg dmg = new Yeet.Dmg(0, 0, 0, 0, 0, 0, 0);
    public Yeet.Dmg dmg2 = new Yeet.Dmg(0, 0, 0, 0, 0, 0, 0);
    public Yeet.Dmg dmg3 = new Yeet.Dmg(0, 0, 0, 0, 0, 0, 0);
    public float muzzleVelocity = 10;

    [NonSerialized]
    public bool defend = true;

    public int maxAmmo = 10000;
    [NonSerialized]
    public int ammo;

    public float recoil = 0.1f;
    public int fireDelay = 10;

    public GameObject x_axis;
    public GameObject z_axis;

    public GameObject barrel;

    public Light flashlight;
    public Light flashlight2;
    public MeshRenderer flashlightRenderer;
    public int flashlightMaterialIndex;
    public Material offMaterial;
    public Material onMaterial;

    public Vector3 maxTurretRot;
    public Vector3 minTurretRot;
    public GameObject targetCrosshair;

    public GameObject targetLaser;
    public GameObject aimLaser;

    public float rotSpeed = 5;

    [NonSerialized]
    public Vector3 turretRot = Vector3.zero;
    [NonSerialized]
    public Vector3 turretRotTarget = Vector3.zero;

    bool flashlights = true;

    bool shooting = false;
    [NonSerialized]
    public int aimMode = 0; // 0 - auto, 1 - 3rd person, 2 - Disabled
    [NonSerialized]
    public int fireMode = 0; // 0 - auto, 1 - Left Click, 2 - Disabled

    public GameObject spinner;
    public GameObject spinner2;
    public Vector3 spinDegreesPerShot;
    public GameObject[] movingParts;
    public int turretType = 0;

    public SoundManager.Sound shootSound;
    public SoundManager.Sound moveSound;
    public SoundManager.Sound foldSound;
    public SoundManager.Sound unfoldSound;
    public SoundManager.Sound impactSount;

    [NonSerialized]
    public Vector3 rotateGun = Vector3.zero;

    float lastFired = 0;

    float speedMult = 1;

    bool playSounds = false;
    //[NonSerialized]
    //public bool targettingDanger = false;

    int gunIndex = -1;
    //public bool usingCamera = false;

    [NonSerialized]
    Vector3 ogCamRot;

    [NonSerialized]
    public bool rotateShip = false;
    // Start is called before the first frame update
    int maxFoldIndex = 1;
    new void Start()
    {
        base.Start();
        if (!stationary)
            turretRot = Vector3.right * x_axis.transform.localEulerAngles.x + Vector3.forward * z_axis.transform.localEulerAngles.z;
        ammo = maxAmmo;
        if (ship == null)
            ship = GetComponentInParent<SpaceShip>();
        if (ship.trophy)
            return;
        if (reactor == null)
            reactor = ship.reactor;
        ogCamRot = cam.transform.localEulerAngles;
        if (turretType == 0)
            maxFoldIndex = 2;
        if (foldable)
        {
            fold = true;
        }
        playSounds = ship.playSounds;
    }

    public void nextAimMode()
    {
        aimMode++;
        aimMode %= 3;
    }
    public void nextFireMode()
    {
        fireMode++;
        fireMode %= 3;
    }


    int c = 0;
    bool cancelShoot = false;
    bool lightsActuallyOn = false;
    bool willHitShip = false;

    new void FixedUpdate()
    {
        if (ship.trophy)
            return;
        base.FixedUpdate();
        if (!playSounds && c % 10 == 0 && ship.playSounds) playSounds = true;
        //Debug.Log("Playsounds = " + playSounds);
        c++;
        c %= 10000;
        if (reactor == null)
        {
            Debug.Log("No Reactor");
            return;
        }
        //Debug.Log("Reactor Found");
        if (c % 30 == 0)
        {
            if (!(reactor.weaponPower && (reactor.power || reactor.batteryPower)))
            {
                TurnOffFlashLight();
            }
        }


        if (reactor.weaponPower && (reactor.power || reactor.batteryPower))
        {
            if (c % 10 == 0)
            {
                speedMult = reactor.power ? (reactor.highPower ? 1.5f : (reactor.lowPower ? 0.5f : 1)) : 1;
                if (flashlight != null)
                {
                    if (flashlights && !folded && !lightsActuallyOn)
                    {
                        LightsOn();
                    }
                    else if ((!flashlights || folded) && lightsActuallyOn)
                    {
                        LightsOff();
                    }
                }
            }
            if ((folded != fold) && foldable)
            {
                if (!reactor.power) reactor.batteryAmount -= batteryConsumption;
                if (fold)
                    FoldSequence(1);
                else
                    FoldSequence(-1);
            }
            if ((folded || fold) && foldable)
                return;
            if (ship.playerShip && ship.player.traceGun)
            {
                drawLine(barrel.transform.position, barrel.transform.position + barrel.transform.up * (100), aimLaser.GetComponent<Renderer>().material);
                //drawLine(barrel.transform.position, barrel.transform.position + targetCrosshair.transform.up * (-100), targetLaser.GetComponent<Renderer>().material);
            }
            else if (lineRenderer != null)
                Destroy(lineRenderer.gameObject);

            if (c % 5 == 0 && !stationary)
            {
                //targets.Remove(null);
                foreach (GameObject o in targets.Keys)
                {
                    if (o == null)
                        targetsToRemove.Add(o);
;                    if (o != null && o.GetComponent<SpaceShip>() != null)
                    {
                        if (!o.GetComponent<SpaceShip>().IsAlive())
                        {
                            targetsToRemove.Add(o);
                            //targets[o] = false;
                            //targets.Remove(o);
                            continue;
                        }
                    }
                    if (o != null && targets[o] && CanReach(o.transform.position))
                    {
                        if (targetLock == null || !CanReach(targetLock.transform.position) || Vector3.Distance(transform.position, o.transform.position) < Vector3.Distance(transform.position, targetLock.transform.position))
                            targetLock = o;
                    }
                }

                if (targetsToRemove.Count > 0)
                {
                    foreach (GameObject g in targetsToRemove)
                    {
                        targets.Remove(g);
                    }
                    targetsToRemove.Clear();
                }

                if (targetLock != null && targetLock.GetComponent<SpaceShip>() != null && !targetLock.GetComponent<SpaceShip>().IsAlive())
                    targetLock = null;

                RaycastHit hit;
                willHitShip = false;
                if (Physics.SphereCast(barrel.transform.position, bulletThickness, barrel.transform.up, out hit, 50, Game.RadarMask))
                {
                    if (hit.transform == ship.transform || hit.transform.IsChildOf(ship.transform) || ship.transform.IsChildOf(hit.transform))
                        willHitShip = true;
                }
            }

            //Debug.Log("Yeet")
;           if (rotateGun.magnitude > 0 && stationary && rotateShip)
            {
                //turretRotTarget = FixAngle(turretRotTarget);
                rotateGun = FixAngle(rotateGun);

                Debug.Log("Rotate Gun " + rotateGun);

                if (rotateGun.z > 0)
                    ship.FireThrusters(ship.d_thrusters, rotateGun.z);
                else if (rotateGun.z < 0)
                    ship.FireThrusters(ship.a_thrusters, -rotateGun.z);

                if (rotateGun.x > 0)
                    ship.FireThrusters(ship.s_thrusters, rotateGun.x);
                else if (rotateGun.x < 0)
                    ship.FireThrusters(ship.w_thrusters, -rotateGun.x);
            }

            if (aimMode == 0 && targetLock != null && ship.activeGun != this)
            {
                if (!stationary)
                {
                    if (!reactor.power) reactor.batteryAmount -= batteryConsumption;
                    AimAt(targetLock, fireMode == 0);
                } else if (stationary)
                {
                    //if (rotateShip)
                    //    ship.TurnToPoint((targetLock.transform.position - ship.transform.position).normalized, ship.transform.InverseTransformDirection(ship.rb.angularVelocity));
                    ShootIfAimingAtTarget(hitscan ? targetLock.transform.position : GetMovingTargetPos(targetLock), 5);
                }
            }

            if (shooting && (stationary || (!cancelShoot && !willHitShip)))
            {
                //Debug.Log("Should shoot, c=" + c + " firedelay=" + fireDelay);
                if (spinner != null)
                    spinner.transform.localEulerAngles += spinDegreesPerShot / fireDelay;
                if (spinner2 != null)
                    spinner2.transform.localEulerAngles += spinDegreesPerShot / fireDelay;
                if ((fireDelay/speedMult < 50 && c % (int)(fireDelay/speedMult) == 0) || (fireDelay/speedMult >= 50 && Time.realtimeSinceStartup / Time.fixedDeltaTime >= lastFired + fireDelay/speedMult))
                {
                    FireBullet(); // Actually shoots the bullet
                    if (fireDelay/speedMult >= 50)
                        lastFired = Time.realtimeSinceStartup / Time.fixedDeltaTime;
                }
            } 

            if (!stationary)
                RotateTurretToTarget();
        }

        if (c % 10 == 0 && Game.instance != null)
        {
            if (Game.instance.record)
            {
                if (turret)
                {
                    if (gunIndex == -1)
                    {
                        for (int i = 0; i < ship.turrets.Length && gunIndex == -1; i++)
                            if (ship.turrets[i] == this)
                                gunIndex = i;
                    }
                    Game.instance.rec.LogTurret(ship, gunIndex, shooting, !shooting, false, false);
                }
            }
        }

        if (!(Game.instance != null && Game.instance.playbackRecording))
            shooting = false;
        cancelShoot = false;
    }

    public void StopShooting()
    {
        shooting = false;
        cancelShoot = true;
    }

    LineRenderer lineRenderer;
    void drawLine(Vector3 a, Vector3 b, Material material)
    {
        //Debug.Log("Drawing line");
        if (lineRenderer == null)
            lineRenderer = new GameObject("GunLaser").AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        Material[] mat = new Material[] { material };
        lineRenderer.materials = mat;
        lineRenderer.material = material;

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer.SetPosition(0, a); //x,y and z position of the starting point of the line
        lineRenderer.SetPosition(1, b); //x,y and z position of the end point of the line
        lineRenderer.receiveShadows = false;
        lineRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer.generateLightingData = false;
        //StartCoroutine(FadeLine(lineRenderer));
    }

    void RotateTurretToTarget()
    {
        if (stationary) return;
        turretRotTarget = FixAngle(turretRotTarget);
        turretRot = FixAngle(turretRot);
        turretRotTarget = new Vector3(Mathf.Clamp(turretRotTarget.x, minTurretRot.x, maxTurretRot.x), 0, Mathf.Clamp(turretRotTarget.z, minTurretRot.z, maxTurretRot.z));

        Vector3 angleDiff = AngleDiff(turretRot, turretRotTarget);

        targetCrosshair.transform.localEulerAngles = -angleDiff.x * Vector3.right + angleDiff.z * Vector3.forward;
        //cam.transform.localEulerAngles = ogCamRot - angleDiff.x * Vector3.right * 0.5f - angleDiff.z * Vector3.forward*0.5f;

        if (angleDiff.magnitude > rotSpeed * speedMult)
        {
            //turretRot = turretRotTarget;
            turretRot += angleDiff.normalized * rotSpeed * speedMult;
        }
        else
        {
            turretRot = turretRotTarget;
        }

        if (playSounds && angleDiff.magnitude > 2)
        {
            moveSound.vol = 0.01f + angleDiff.magnitude * 0.075f / 180;
            moveSound.pitch = 0.7f + angleDiff.magnitude * 0.5f / 180;
            moveSound.play(transform.position);
        }

        x_axis.transform.localEulerAngles = new Vector3(turretRot.x, x_axis.transform.localEulerAngles.y, x_axis.transform.localEulerAngles.z);
        z_axis.transform.localEulerAngles = new Vector3(z_axis.transform.localEulerAngles.x, z_axis.transform.localEulerAngles.y, turretRot.z);
    }

    int foldIndex = -1;
    void FoldSequence(int dir)
    {
        if (!foldable) return;
        if (Game.instance != null && Game.instance.record)
        {
            if (turret)
            {
                if (gunIndex == -1)
                {
                    for (int i = 0; i < ship.turrets.Length && gunIndex == -1; i++)
                        if (ship.turrets[i] == this)
                            gunIndex = i;
                }
                Game.instance.rec.LogTurret(ship, gunIndex, false, true, foldIndex == 0 && dir > 0, foldIndex == maxFoldIndex && dir < 0);
            }
        }
        if (foldIndex == -1)
        {
            if (folded == true)
                foldIndex = maxFoldIndex;
            else if (folded == false)
                foldIndex = 0;
        }
        if (turretType == 1)
        {
            bool c = true;/*
            Debug.Log("FoldIndex = " + foldIndex);
            Debug.Log("Folded = " + folded);
            Debug.Log("Fold = " + fold);
            Debug.Log("Dir = " + dir);*/
            switch (foldIndex)
            {
                case 0: // Unfolded Position
                    c = TranslateStep(movingParts[0], new Vector3(0, 0, -0.001f), 0.01f) && c;
                    c = TranslateStep(movingParts[1], new Vector3(0, -0.0108f, -0.0128f), 0.01f) && c;
                    c = TranslateStep(movingParts[2], new Vector3(0, 0.0108f, -0.0128f), 0.01f) && c;
                    c = RotateStep(movingParts[1], new Vector3(90, 0, 0), 5) && c;
                    //c = c && RotateStep(movingParts[2], new Vector3(-90, 0, 0), 5);
                    c = RotateStep(movingParts[2], new Vector3(270, 0, 0), 5) && c;
                    c = AimLocal(new Vector3(0, 1.41f, 0f)) && c;
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
                case 1: // Slide Doors upwardsand turret downwards
                    c = TranslateStep(movingParts[0], new Vector3(0, 0, -0.0157f), 0.01f) && c;
                    c = TranslateStep(movingParts[1], new Vector3(0, -0.0108f, -0.0003632093f), 0.01f) && c;
                    c = TranslateStep(movingParts[2], new Vector3(0, 0.0108f, -0.0003632093f), 0.01f) && c;
                    c = RotateStep(movingParts[1], new Vector3(90, 0, 0), 5) && c;
                    //c = RotateStep(movingParts[2], new Vector3(-90, 0, 0), 5) && c;
                    c = RotateStep(movingParts[2], new Vector3(270, 0, 0), 5) && c;
                    c = AimLocal(new Vector3(0, 1.41f, 0f)) && c;
                    if (c)
                        foldIndex += dir;
                    break;
                case 2: // Close Doors
                    c = TranslateStep(movingParts[0], new Vector3(0, 0, -0.0157f), 0.01f) && c;
                    c = TranslateStep(movingParts[1], new Vector3(0, -0.0108f, -0.0003632093f), 0.01f) && c;
                    c = TranslateStep(movingParts[2], new Vector3(0, 0.0108f, -0.0003632093f), 0.01f) && c;
                    c = RotateStep(movingParts[1], new Vector3(0, 0, 0), 5) && c;
                    c = RotateStep(movingParts[2], new Vector3(359, 0, 0), 5) && c;
                    c = AimLocal(new Vector3(0, 1.41f, 0f)) && c;
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

    bool RotateStep(GameObject obj, Vector3 rot, float speed)
    {
        if (Vector3.Distance(obj.transform.localEulerAngles, rot) < speed * 1.5f)
        {
            obj.transform.localEulerAngles = rot;
            return true;
        }
        else
            obj.transform.localEulerAngles += (rot - obj.transform.localEulerAngles).normalized * speed;
        //Debug.Log(obj.name + ", TargetRot = " + rot + ", CurrentRot = " + obj.transform.localEulerAngles + ", Remainder = " + Vector3.Distance(obj.transform.localEulerAngles, rot));
        return false;
    }

    bool TranslateStep(GameObject obj, Vector3 trans, float speed)
    {
        speed *= 0.05f;
        if (Vector3.Distance(obj.transform.localPosition, trans) < speed * 1.5f)
        {
            obj.transform.localPosition = trans;
            return true;
        }
        else
            obj.transform.localPosition += (trans - obj.transform.localPosition).normalized * speed;
        return false;
    }

    Vector3 FixAngle(Vector3 eulers)
    {
        while (eulers.x > 180)
            eulers.x -= 360;
        while (eulers.x < -180)
            eulers.x += 360;
        while (eulers.y > 180)
            eulers.y -= 360;
        while (eulers.y < -180)
            eulers.y += 360;
        while (eulers.z > 180)
            eulers.z -= 360;
        while (eulers.z < -180)
            eulers.z += 360;
        return eulers;
    }

    Vector3 AngleDiff(Vector3 a, Vector3 b)
    {
        float x;
        float y;
        float z;

        x = b.x - a.x;
        if (Mathf.Abs(x) > Mathf.Abs(b.x - a.x + 360))
            x = b.x - a.x + 360;
        if (Mathf.Abs(x) > Mathf.Abs(b.x - a.x - 360))
            x = b.x - a.x - 360;

        y = b.y - a.y;
        if (Mathf.Abs(y) > Mathf.Abs(b.y - a.y + 360))
            y = b.y - a.y + 360;
        if (Mathf.Abs(y) > Mathf.Abs(b.y - a.y - 360))
            y = b.y - a.y - 360;

        z = b.z - a.z;
        if (Mathf.Abs(z) > Mathf.Abs(b.z - a.z + 360))
            z = b.z - a.z + 360;
        if (Mathf.Abs(z) > Mathf.Abs(b.z - a.z - 360))
            z = b.z - a.z - 360;

        return new Vector3(x, y, z);
    }


    bool CanReach(Vector3 pos)
    {
        Vector3 posLocal = transform.InverseTransformPoint(pos) - transform.InverseTransformPoint(barrel.transform.position);
        RaycastHit hit;
        if (Physics.SphereCast(barrel.transform.position, bulletThickness*1.5f, pos - barrel.transform.position, out hit, 100, Game.RadarMask))
        {
            if (hit.transform == ship.transform || hit.transform.IsChildOf(ship.transform) || ship.transform.IsChildOf(hit.transform))
                return false;
        }
        float z = Mathf.Atan2(-posLocal.x, posLocal.y) * 180 / Mathf.PI;
        float x = Mathf.Asin(posLocal.z / posLocal.magnitude) * 180 / Mathf.PI;
        if (x > minTurretRot.x && x < maxTurretRot.x && z > minTurretRot.z && z < maxTurretRot.z)
            return true;
        return false;
    }

    bool AimLocal(Vector3 posLocal, float tol = 10)
    {
        if (stationary) return true;
        float z = Mathf.Atan2(-posLocal.x, posLocal.y) * 180 / Mathf.PI;
        float x = Mathf.Asin(posLocal.z/posLocal.magnitude) * 180 / Mathf.PI;

        Vector3 idealTurretRot = new Vector3(x, 0, z);
        turretRotTarget = new Vector3(x, 0, z);

        RotateTurretToTarget();

        return AimingAtTargetLocal(posLocal, tol);
    }

    public void AimAt(Vector3 pos, bool autoShoot)
    {
        Vector3 posLocal = transform.InverseTransformPoint(pos) - transform.InverseTransformPoint(barrel.transform.position);
        //Instantiate(muzzleFlash, transform.TransformPoint(posLocal), Quaternion.Euler(Vector3.zero));

        //posLocal = posLocal.normalized;
        //Debug.Log("Point to " + posLocal);

        float z = Mathf.Atan2(-posLocal.x, posLocal.y) * 180 / Mathf.PI;
        float x = Mathf.Asin(posLocal.z/posLocal.magnitude) * 180 / Mathf.PI;

        Vector3 idealTurretRot = new Vector3(x, 0, z);
        turretRotTarget = new Vector3(x, 0, z);
        //turretRotTarget = Quaternion.FromToRotation(Vector3.up, posLocal).eulerAngles;

        if ((autoShoot) && fireMode == 0 && AimingAtTarget(pos))
        {
            if (autoShoot)
                Shoot();
        }
    }

    public void AimAt(GameObject target, bool autoShoot)
    {

        AimAt(GetMovingTargetPos(target), autoShoot);
    }

    Vector3 GetMovingTargetPos(GameObject target)
    {
        if (hitscan)
            return target.transform.position;
        Vector3 targetVel = Vector3.zero;
        Vector3 targetAcc = Vector3.zero;
        if (target.GetComponent<Rigidbody>() != null)
        {
            targetVel = target.GetComponent<Rigidbody>().velocity;
            if (targetVel.magnitude > Game.instance.maxMissileSpeed)
                targetVel = targetVel.normalized * Game.instance.maxMissileSpeed;
            if (targetVel.magnitude < Game.instance.maxMissileSpeed*0.95f)
                targetAcc = target.GetComponent<Rigidbody>().GetAccumulatedForce() / target.GetComponent<Rigidbody>().mass;
        }
        Vector3 velDiff = targetVel - ship.rb.velocity;
        float time = hitscan ? Time.fixedDeltaTime : Vector3.Distance(target.transform.position, transform.position) / muzzleVelocity;
        velDiff += targetAcc * time;

        time = Vector3.Distance(target.transform.position + velDiff * time, transform.position) / muzzleVelocity;
        time = Vector3.Distance(target.transform.position + velDiff * time, transform.position) / muzzleVelocity;

        Vector3 targetPos = target.transform.position + velDiff * time;

        if (Vector3.Angle(transform.position - target.transform.position, velDiff) < 10 && Vector3.Angle(targetPos, target.transform.position) > 45)
        {
            targetPos = target.transform.position;
        }

        //Debug.Log("vel=" + targetVel + ", acc=" + targetAcc + ", velDiff=" + velDiff);

        return targetPos;
    }

    bool CanReach(GameObject target)
    {
        return CanReach(GetMovingTargetPos(target));
    }

    public bool AimingAtTarget(Vector3 pos, float tol = 15)
    {
        Vector3 posLocal = barrel.transform.InverseTransformPoint(pos);
        //Debug.Log(Vector3.Angle(pos, barrel.transform.up));
        return Vector3.Angle(posLocal, Vector3.up) < tol;
    }

    public bool AimingAtTargetLocal(Vector3 posLocal, float tol = 15)
    {
        return Vector3.Angle(posLocal, Vector3.up) < tol;
    }

    void FireBullet()
    {
        //Debug.Log("Firing Bullet");
        if (this.bullet == null) return;

        if (ship != null)
        {
            ship.electricNoise += shootNoise;
        }

        if (!reactor.power) reactor.batteryAmount -= batteryConsumption * 5;
        if (playSounds)
        {
            Debug.Log("playing sound");
            shootSound.play(barrel.transform.position);
        }

        GameObject bullet = this.bullet;
        GameObject muzzleFlash = this.muzzleFlash;

        if (bullet2 != null && bullet3 != null)
        {
            switch ((int)(UnityEngine.Random.Range(0, 3)))
            {
                case 0:
                    bullet = this.bullet;
                    if (this.muzzleFlash != null) muzzleFlash = this.muzzleFlash2;
                    break;
                case 1:
                    bullet = this.bullet2;
                    if (this.muzzleFlash2 != null) muzzleFlash = this.muzzleFlash2;
                    break;
                case 2:
                    bullet = this.bullet3;
                    if (this.muzzleFlash3 != null) muzzleFlash = this.muzzleFlash3;
                    break;
            }
        }

        //Debug.Log("Shooting");
        GameObject temp = GameObject.Instantiate(bullet, barrel.transform) as GameObject;
        GameObject temp2 = GameObject.Instantiate(muzzleFlash, barrel.transform) as GameObject;
        //temp2.transform.position = barrel.transform.position + ship.rb.velocity * Time.fixedDeltaTime;
        //temp.transform.position = barrel.transform.position + ship.rb.velocity*Time.fixedDeltaTime;
        //temp.transform.localPosition = ship.rb.velocity * Time.fixedDeltaTime;
        //temp2.transform.localPosition = ship.rb.velocity * Time.fixedDeltaTime;
        temp.transform.localPosition = Vector3.zero;
        temp2.transform.localPosition = Vector3.zero;
        temp.transform.parent = null;
        temp2.transform.parent = null;
        temp.transform.localScale = bullet.transform.localScale;
        temp2.transform.localScale = muzzleFlash.transform.localScale;
        temp.transform.position += ship.rb.velocity * Time.fixedDeltaTime;
        temp2.transform.position += ship.rb.velocity * Time.fixedDeltaTime;
        //temp.transform.eulerAngles = barrel.transform.eulerAngles - Vector3.right*90;
        Particle tempParticle = temp.GetComponent<Particle>();
        Particle tempParticle2 = temp2.GetComponent<Particle>();
        tempParticle.impactSound = impactSount;
        tempParticle.dmg = dmg;
        tempParticle.forMissile = true;
        if (tempParticle.hitscan)
        {
            tempParticle.velocity = ship.rb.velocity;
            StartCoroutine(tempParticle.HitscanFireRoutine());
        }
        tempParticle.velocity = barrel.transform.up * muzzleVelocity + ship.rb.velocity;
        if (turretType == 0)
            tempParticle2.velocity = barrel.transform.up * muzzleVelocity*0.2f + ship.rb.velocity;
        else if (turretType == 1)
            tempParticle2.velocity = ship.rb.velocity;

        ship.rb.AddForceAtPosition(-barrel.transform.up * recoil, barrel.transform.position, ForceMode.Impulse);
        ammo--;
    }

    public void Shoot()
    {
        //Debug.Log("FIRE!!!");
        shooting = true;
    }

    public void ShootIfAimingAtTarget(Vector3 pos, float tol = 15)
    {
        if (AimingAtTarget(pos, tol)) shooting = true;
    }

    void LightsOn()
    {
        if (flashlight != null)
        {
            flashlight.enabled = true;
            flashlight.gameObject.SetActive(true);
            Material[] mats = flashlightRenderer.materials;
            mats[flashlightMaterialIndex] = onMaterial;
            flashlightRenderer.materials = mats;
            lightsActuallyOn = true;
        }
        if (flashlight2 != null)
        {
            flashlight2.enabled = true;
            flashlight2.gameObject.SetActive(true);
        }
    }

    void LightsOff()
    {

        if (flashlight != null)
        {
            flashlight.enabled = false;
            flashlight.gameObject.SetActive(false);
            Material[] mats = flashlightRenderer.materials;
            mats[flashlightMaterialIndex] = offMaterial;
            flashlightRenderer.materials = mats;
            lightsActuallyOn = false;
        }
        if (flashlight2 != null)
        {
            flashlight2.enabled = false;
            flashlight2.gameObject.SetActive(false);
        }
    }

    public void TurnOnFlashLight()
    {
        //LightsOn();
        flashlights = true;
    }

    public void TurnOffFlashLight()
    {
        //LightsOff();
        flashlights = false;
    }

    public bool flashLightStatus()
    {
        return flashlights;
    }
}
