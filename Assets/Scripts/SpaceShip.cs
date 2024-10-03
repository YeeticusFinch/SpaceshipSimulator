using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class SpaceShip : SpaceObject
{
    public string description;
    [NonSerialized]
    public bool trophy = false; // set to true if you want this ship to just be a dummy, to not do anything at all
    [NonSerialized]
    public float electricNoise = 0;
    public float defaultStabalizerPower;
    public float defaultKp;
    public float defaultSpeedTol;
    public float defaultAngleTol;
    public float defaultPointToKp;
    public float defaultPointToKd;
    //[NonSerialized]
    public int team = 0;

    public MomentumWheelAssembly momentumWheels;

    public static int numShips = 0;
    [NonSerialized]
    public int shipUUID = 0;

    [NonSerialized]
    public bool playSounds = false;

    public Camera PilotEyes;
    public RenderTexture PilotScreen;

    [NonSerialized]
    public bool playerShip;
    [NonSerialized]
    public bool npcShip;
    [NonSerialized]
    public PlayerShip player;

    public Cockpit cockpit;
    public Reactor reactor;
    public ShipObject[] hull;
    public gun[] turrets;
    public gun[] statGuns;
    public MissileLauncher[] launchers;
    public Radar[] radars;
    public Targeter[] targeters;

    public GameObject thirdPersonCamera;
    public GameObject thirdPersonAxis;
    public ShipObject[] cameras;

    public Thruster mainDrive;
    public Thruster teaKettle;
    public Thruster misc;
    public Thruster misc2;

    public Thruster[] w_thrusters;
    public Thruster[] a_thrusters;
    public Thruster[] d_thrusters;
    public Thruster[] s_thrusters;
    public Thruster[] space_thrusters;
    public Thruster[] shift_thrusters;
    public Thruster[] q_thrusters;
    public Thruster[] e_thrusters;

    public Thruster[] alt_w_thrusters;
    public Thruster[] alt_s_thrusters;
    public Thruster[] alt_a_thrusters;
    public Thruster[] alt_d_thrusters;
    public Thruster[] alt_space_thrusters;
    public Thruster[] alt_shift_thrusters;

    [NonSerialized]
    public bool jamTargetters = false;
    [NonSerialized]
    public bool jamRadars = false;

    [NonSerialized]
    public bool radarOn = true;

    [NonSerialized]
    public bool targetted = false;

    [NonSerialized]
    public gun activeGun = null;

    [NonSerialized]
    public bool overrideW = false;
    [NonSerialized]
    public bool overrideS = false;
    [NonSerialized]
    public bool overrideA = false;
    [NonSerialized]
    public bool overrideD = false;
    [NonSerialized]
    public bool overrideSpace = false;
    [NonSerialized]
    public bool overrideShift = false;
    [NonSerialized]
    public bool overrideQ = false;
    [NonSerialized]
    public bool overrideE = false;
    [NonSerialized]
    public bool overrideAlt = false;

    [NonSerialized]
    public bool stabalizers = true;

    [NonSerialized]
    public float stabalizerPower = 0.8f;

    //[NonSerialized]
    //public Rigidbody rb;

    [NonSerialized]
    public Vector3 thirdPersonRot;
    [NonSerialized]
    public Vector3 thirdPersonTrans;

    [NonSerialized]
    public Vector3 acceleration;

    [NonSerialized]
    public float safeDistance = 50;

    public bool hasInterceptLaunchers = false;

    [NonSerialized]
    public bool useInterceptMissiles = false;

    int activeTargeters = 0;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (trophy) return;
        stabalizerPower = defaultStabalizerPower;
        Kp = defaultKp;
        speedTol = defaultSpeedTol;
        angleTol = defaultAngleTol;
        pointToKp = defaultPointToKp;
        pointToKd = defaultPointToKd;
        shipUUID = numShips;
        numShips++;
        isShip = true;
        ConfigThrusters();
        //rb = GetComponent<Rigidbody>();
        thirdPersonRot = Vector3.zero;
        thirdPersonTrans = Vector3.zero;
        name = Yeet.ShipNames[Random.Range(0, Yeet.ShipNames.Length)];
    }

    void ConfigThrusters()
    {
        foreach (Thruster t in transform.GetComponentsInChildren<Thruster>())
        {
            if (t.mainDrive)
                t.GrabValues(mainDrive);
            else if (t.teaKettle)
                t.GrabValues(teaKettle);
            else if (t.misc)
                t.GrabValues(misc);
            else if (t.misc2)
                t.GrabValues(misc2);
        }
    }

    public int rebootingDrive = 0;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.GetComponent<Missile>() != null)
            return;
        else
            base.OnCollisionEnter(collision);
    }

    public void rebootPropulsion()
    {
        
        pressedKeys = new List<KeyCode>();
        stabalizerPower = 0;
        speedTol = 0;
        angleTol = 0;
        Kp = 0;
        useMainDriveForStabalization = false;
        useStabalizersForMainThrust = false;
        stabalizers = false;
        flip_n_burn = false;
        rebootingDrive = 5;
        reactor.drivePower = false;
    }

    void ResetDrive()
    {
        pressedKeys = new List<KeyCode>();
        stabalizerPower = defaultStabalizerPower;
        Kp = defaultKp;
        speedTol = defaultSpeedTol;
        angleTol = defaultAngleTol;
        pointToKp = defaultPointToKp;
        pointToKd = defaultPointToKd;
        useMainDriveForStabalization = false;
        useStabalizersForMainThrust = false;
        stabalizers = true;
        flip_n_burn = false;
        reactor.drivePower = true;
    }

    public float speedTol = 0.014f;
    public float angleTol = 2;

    public Vector3 velRef = Vector3.zero;

    public bool useMainDriveForStabalization = false;
    public bool useStabalizersForMainThrust = false;
    public bool flip_n_burn = false;
    [NonSerialized]
    public Vector3 goToPoint = Vector3.zero;
    [NonSerialized]
    public GameObject pointTo = null;
    [NonSerialized]
    public float flip_n_burn_time = 10;
    Vector3 goToPointOld = Vector3.zero;
    float ogDist = 0;
    float maxProjVel = 0;

    [NonSerialized]
    public List<KeyCode> pressedKeys = new List<KeyCode>();

    private Vector3 prev_velocity;

    public bool CheckMovementKeys()
    {
        foreach (KeyCode k in PlayerShip.MOVEMENT_KEYS)
        {
            if (pressedKeys.Contains(k))
                return true;
        }
        return false;
    }
    
    //float prevAngle = -1;
    //int angleDir = 0;
    float Angle(Vector3 a, Vector3 b, float vel, float[] yeet)
    {
        float prevAngle = yeet[0];
        float angleDir = yeet[1];
        float angle = Vector3.Angle(a, b);
        float result = angle;
        Debug.Log("Velocity: " + vel);
        if (prevAngle == -1)
        {
            prevAngle = angle;
        } else if (Mathf.Abs(angleDir) < 0.01f)
        {
            angleDir = (int)Mathf.Sign(angle - prevAngle);
            prevAngle = angle;
        }
        else
        {
            result = prevAngle + angleDir * vel;
            prevAngle = result;
        }

        yeet[0] = prevAngle;
        yeet[1] = angleDir;

        return result;
    }

    public void cameraZoom(float amount)
    {
        if (thirdPersonCamera)
        {
            thirdPersonCamera.transform.localPosition += amount * Vector3.forward;
        }
    }

    public bool IsAlive()
    {
        bool hullAlive = false;
        bool humanAlive = false;
        foreach (ShipObject h in hull)
        {
            if (h != null && h.gameObject != null && h.enabled && h.gameObject.active && h.GetHP() > 0.01f)
                hullAlive = true;
        }
        foreach (Human h in cockpit.people)
        {
            if (h != null && h.gameObject != null && h.enabled && h.gameObject.active && h.GetHP() > 0.01f)
                humanAlive = true;
        }
        return hullAlive && humanAlive;
    }

    public Vector3 GetThirdPersonTarget()
    {
        return thirdPersonCamera.transform.position + thirdPersonCamera.transform.forward * aimDistance;
    }

    public void ChangeTargetLockStatus(GameObject o, bool status)
    {
        foreach (gun g in turrets)
        {
            if (!status && g.targetLock != null && (g.targetLock.Equals(o) || Vector3.Distance(g.targetLock.transform.position, o.transform.position) < 0.1f))
                g.targetLock = null;
            if (g.targets.Count > 0)
            {
                GameObject[] tempKeys = new GameObject[g.targets.Keys.Count];
                g.targets.Keys.CopyTo(tempKeys, 0);
                if (g.targets.ContainsKey(o))
                    g.targets[o] = status;
                foreach (GameObject e in tempKeys)
                {
                    if (e != null && (e.Equals(o) || Vector3.Distance(e.transform.position, o.transform.position) < 0.1f))
                        g.targets[e] = status;
                }
            }
        }
    }

    public void ClearTargetLock(GameObject o)
    {
        Debug.Log("G");
        foreach (gun g in turrets)
        {
            if (g.targetLock != null && (g.targetLock.Equals(o) || Vector3.Distance(g.targetLock.transform.position, o.transform.position) < 0.1f))
                g.targetLock = null;
            if (g.targets.Count > 0)
            {
                g.targets.Remove(o);
                //foreach (GameObject e in g.targets)
                //{
                //    if (e != null && (e.Equals(o) || Vector3.Distance(e.transform.position, o.transform.position) < 0.1f))
                //        g.targets = null;
                //}
                //g.targets.Remove(null);
            }
        }
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("UI Target"))
        {
            if (g.GetComponent<Indicator>().target.Equals(o) || Vector3.Distance(o.transform.position, g.GetComponent<Indicator>().target.transform.position) < 0.1f)
            {
                Debug.Log("EE");
                Destroy(g);
            }
        }
    }

    public void ClearTargetLock(gun g)
    {
        foreach(GameObject o in GameObject.FindGameObjectsWithTag("UI Target"))
        {
            if (o.GetComponent<Indicator>().target.Equals(g.targetLock) || Vector3.Distance(g.targetLock.transform.position, o.GetComponent<Indicator>().target.transform.position) < 0.1f)
            {
                Debug.Log("E");
                Destroy(o);
            }
        }
        g.targetLock = null;
        if (g.targets.Count > 0)
        {
            Debug.Log("F");
            g.targets.Clear();
        }
    }

    void CalculateElectricNoise()
    {
        electricNoise = 0;
        if (reactor != null && reactor.alive() && reactor.power)
        {
            if (reactor.lowPower)
                electricNoise += reactor.electricNoise / 2;
            else if (reactor.highPower)
                electricNoise += reactor.electricNoise * 2;
            else
                electricNoise += reactor.electricNoise;
        }

        if (reactor.power || reactor.batteryPower)
        {
            if (reactor.weaponPower)
            {
                foreach (gun g in turrets)
                {
                    if (!(g.foldable && g.folded))
                        electricNoise += g.electricNoise;
                }
                foreach (gun g in statGuns)
                {
                    if (!(g.foldable && g.folded))
                        electricNoise += g.electricNoise;
                }
                foreach (MissileLauncher g in launchers)
                {
                    electricNoise += g.electricNoise;
                }
            }

            if (reactor.telemetryPower)
            {
                if (radarOn)
                {
                    foreach (Radar r in radars)
                    {
                        if (r != null && r.enabled && r.gameObject.activeSelf)
                            electricNoise += r.electricNoise;
                    }
                }
                if (targeters.Length > 0)
                {
                    foreach (Targeter t in targeters)
                    {
                        if (t != null)
                        {
                            electricNoise += activeTargeters * t.electricNoise;
                            break;
                        }
                    }
                }
            }
            if (reactor.commsPower)
            {

            }
            if (reactor.drivePower)
            {

            }
        }
    }

    int c = 0;
    // Update is called once per frame
    [NonSerialized]
    public float mainDrivePower = 1;
    [NonSerialized]
    public float maneuveringPower = 1;
    bool movementKeysPressed = false;
    float step_input = 0f;
    bool reverseStep = false;
    float[] stepAngle = { -1, 0 };
    [NonSerialized]
    public float aimDistance = 10;
    [NonSerialized]
    public bool dangerObjectsPresent = false;
    void FixedUpdate()
    {
        if (trophy) return;
        base.FixedUpdate();
        if (!playSounds && c % 50 == 0 && (Game.instance != null && Game.instance.atmosphere)) playSounds = true;
        if (Game.instance != null)
        {
            if (rb.velocity.magnitude > Game.instance.maxShipSpeed)
                rb.velocity = rb.velocity.normalized * Game.instance.maxShipSpeed;
        }
        c++;
        c %= 1000;

        thirdPersonAxis.transform.eulerAngles = thirdPersonRot;
        thirdPersonAxis.transform.position = transform.position + thirdPersonTrans;
        Vector3 relVelGlob = rb.velocity - velRef;
        Vector3 relVel = transform.InverseTransformDirection(relVelGlob);
        Vector3 relRot = transform.InverseTransformDirection(rb.angularVelocity);

        if (Game.instance != null && Game.instance.playbackRecording)
            return;

        if (c % 40 == 0)
            CalculateElectricNoise();

        if (rebootingDrive > 0)
        {
            if (c % 30 == 0)
                rebootingDrive--;
            if (rebootingDrive == 0)
            {
                ResetDrive();
            }
            return;
        }
        if (!IsAlive())
        {
            if (thirdPersonAxis.transform.parent != null)
                thirdPersonAxis.transform.parent = null;
            thirdPersonAxis.SetActive(true);
            thirdPersonCamera.SetActive(true);
            return;
        }
        //Debug.Log("Angle: " + Angle(Vector3.up, transform.up, rb.angularVelocity.magnitude*Time.fixedDeltaTime));
        //Debug.Log("Angle: " + (transform.eulerAngles));
        if (c % 10 == 0)
        {
            Vector3 new_velocity = rb.velocity;
            if (prev_velocity != null)
            {
                acceleration = (new_velocity - prev_velocity) / (10 * Time.fixedDeltaTime);
            }
            prev_velocity = new_velocity;
            movementKeysPressed = CheckMovementKeys();
        }

        if (c%50 == 0)
        {
            GameObject[] circles = GameObject.FindGameObjectsWithTag("UI Circle");
            List<GameObject> hasCircle = new List<GameObject>();
            foreach (GameObject g in circles)
            {
                if (g.GetComponent<Indicator>().target != null)
                {
                    hasCircle.Add(g.GetComponent<Indicator>().target);
                }
            }
            if (radarOn && reactor.telemetryPower)
            {
                foreach (SpaceObject o in GameObject.FindObjectsOfType(typeof(SpaceObject)))
                {
                    if (o.gameObject == gameObject)
                        continue;
                    if ((npcShip || (playerShip && !player.radarObjects)) && ((o.GetComponent<SpaceShip>() == null && o.GetComponent<Missile>() == null && o.GetComponent<SimpleMotionObject>() == null) || (o.GetComponent<SpaceShip>() != null && o.GetComponent<SpaceShip>().IsAlive() == false)))
                    {
                        if (hasCircle.Contains(o.gameObject))
                        {
                            for (int i = 0; i < circles.Length; i++)
                            {
                                if (circles[i].GetComponent<Indicator>().target == o.gameObject)
                                {
                                    //Debug.Log("A");
                                    Destroy(circles[i]);
                                }
                            }
                        }
                        continue;
                    }
                    bool radared = false;
                    dangerObjectsPresent = false;
                    foreach (Radar r in radars)
                    {
                        if (r != null && r.enabled && r.gameObject.activeSelf && r.RadarObject(o.gameObject, playerShip && player.traceRadar))
                        {
                            radared = true;
                            if (playerShip)
                            {
                                if (!hasCircle.Contains(o.gameObject))
                                {
                                    GameObject temp = Instantiate(player.circle, player.canvas.transform);
                                    temp.transform.localEulerAngles = Vector3.zero;
                                    temp.GetComponent<Indicator>().cam = player.cam;
                                    temp.GetComponent<Indicator>().target = o.gameObject;
                                    temp.GetComponent<Indicator>().dist = player.canvas.planeDistance;
                                    temp.GetComponent<Indicator>().trackObject = true;
                                    temp.GetComponentInChildren<TextMeshProUGUI>().text = o.name;
                                    temp.tag = "UI Circle";
                                } else
                                {
                                    bool foundCircle = false;
                                    for (int i = 0; i < circles.Length; i++)
                                    {
                                        if (circles[i].GetComponent<Indicator>().target == o.gameObject)
                                        {
                                            if (foundCircle)
                                            {
                                                Destroy(circles[i]);
                                                //Debug.Log("BB");
                                            }
                                            foundCircle = true;
                                            circles[i].GetComponent<Indicator>().guess = false;
                                            circles[i].GetComponent<Image>().color = Color.white;
                                            circles[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.white;

                                        }
                                    }
                                }
                                if (player.traceMissile && o.GetComponent<Missile>() != null)
                                {
                                    o.GetComponent<Missile>().trace = true;
                                }
                            }

                            bool dangerObject = false;

                            if (Vector3.Distance(o.transform.position, transform.position) > 1f && o.gameObject.GetComponent<Rigidbody>() != null)
                            {
                                Vector3 velDiff = o.gameObject.GetComponent<Rigidbody>().velocity - rb.velocity;

                                if ((o.gameObject.tag == "AlwaysTarget" || (o.gameObject.GetComponent<SimpleMotionObject>() != null && o.gameObject.GetComponent<SimpleMotionObject>().alwaysTarget)) || (o.gameObject.GetComponent<Missile>() != null && o.gameObject.GetComponent<Missile>().target != null && Vector3.Distance(o.gameObject.GetComponent<Missile>().target.transform.position, transform.position) < o.gameObject.GetComponent<Missile>().explodeRange && o.gameObject.GetComponent<Missile>().chasing) || (Vector3.Angle(transform.position - o.gameObject.transform.position, velDiff) < 10 && o.gameObject.GetComponent<Rigidbody>().velocity.magnitude > 5f && Vector3.Distance(transform.position, o.transform.position) < safeDistance))
                                {
                                    dangerObjectsPresent = true;
                                    foreach (Targeter t in targeters)
                                    {
                                        if (t.TargetObject(o.gameObject, playerShip && player.traceRadar))
                                        {
                                            dangerObject = true;
                                            //Debug.Log("Is Dangerous " + o.name);
                                            foreach (gun g in turrets)
                                            {
                                                if (g.defend && !g.targets.ContainsKey(o.gameObject))
                                                {
                                                    g.fold = false;
                                                    g.targets.Add(o.gameObject, true);
                                                }
                                            }
                                            bool missileAlreadyLanched = false;
                                            foreach (Missile m in GameObject.FindObjectsOfType(typeof(Missile)))
                                            {
                                                if (m.owner == this && m.target == o.gameObject) missileAlreadyLanched = true;
                                            }
                                            if (!missileAlreadyLanched)
                                            {
                                                foreach (MissileLauncher l in launchers)
                                                {
                                                    if (l.defend || (useInterceptMissiles && l.intercept))
                                                    {
                                                        l.defend = false;
                                                        l.Launch(o.gameObject);
                                                    }
                                                }
                                            }
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!dangerObject)
                            {
                                //Debug.Log("Not Dangerous " + o.name);
                                bool hasTarget = false;
                                foreach (GameObject e in GameObject.FindGameObjectsWithTag("UI Target"))
                                    if (e.GetComponent<Indicator>().target == o.gameObject)
                                        hasTarget = true;
                                if (!hasTarget)
                                {
                                    //Debug.Log("No Target " + o.name);
                                    foreach (gun g in turrets)
                                    {
                                        if (g.defend)
                                        {
                                            if (g.targets.ContainsKey(o.gameObject))
                                                g.targets.Remove(o.gameObject);
                                            if (g.targetLock == o.gameObject)
                                                g.targetLock = null;
                                        }
                                    }
                                }
                            }
                            
                        }
                        if (radared) break;
                    }
                    if (!radared && hasCircle.Contains(o.gameObject))
                    {
                        bool foundCircle = false;
                        for (int i = 0; i < circles.Length; i++)
                        {
                            if (circles[i].GetComponent<Indicator>().target == o.gameObject)
                            {
                                if (foundCircle)
                                {
                                    Destroy(circles[i]);
                                    Debug.Log("B");
                                }
                                foundCircle = true;
                                circles[i].GetComponent<Indicator>().guess = true;
                                circles[i].GetComponent<Image>().color = Color.yellow;
                                circles[i].GetComponentInChildren<TextMeshProUGUI>().color = Color.yellow;
                                circles[i].GetComponent<Indicator>().guessPos = o.gameObject.transform.position;
                                if (o.gameObject.GetComponent<Rigidbody>() != null)
                                {
                                    circles[i].GetComponent<Indicator>().guessVel = o.gameObject.GetComponent<Rigidbody>().velocity;
                                }
                                else
                                    circles[i].GetComponent<Indicator>().guessVel = Vector3.zero; ;
                                //Destroy(circles[i]);
                            }
                        }
                    }
                }
            }
            if (reactor.telemetryPower && playerShip)
            {
                activeTargeters = 0;
                foreach (GameObject o in GameObject.FindGameObjectsWithTag("UI Target"))
                {
                    bool stillInRange = false;
                    Indicator indi = o.GetComponent<Indicator>();
                    foreach (Targeter t in targeters)
                    {
                        if (playerShip && t.TargetObject(indi.target, player.traceRadar))
                        {
                            activeTargeters++;
                            stillInRange = true;
                                break;
                        }
                    }
                    //if (!indi.guess)
                    //{
                        if (stillInRange == false && !indi.guess) // Lost target lock on object o
                        {
                            //if (o.GetComponent<Indicator>().target != null)
                            //    ClearTargetLock(o.GetComponent<Indicator>().target);
                            //Destroy(o);
                            ChangeTargetLockStatus(indi.target, false);
                            Vector3 lastVel = Vector3.zero;
                            if (indi.target.GetComponent<Rigidbody>() != null)
                                lastVel = indi.target.GetComponent<Rigidbody>().velocity;
                            else if (indi.target.GetComponentInParent<Rigidbody>() != null)
                                lastVel = indi.target.GetComponentInParent<Rigidbody>().velocity;
                            ConvertTargetLockIndicatorStatus(o, false, indi.target.transform.position, lastVel);
                        Debug.Log("C");
                        }
                    //} else
                    //{
                        if (stillInRange == true && indi.guess) // Regained target lock on object o
                        {
                            ChangeTargetLockStatus(indi.target, true);
                            ConvertTargetLockIndicatorStatus(o, true);
                        }
                    //}
                }
            }
        }

        if (c % 2 == 0)
        {
            foreach (gun t in turrets)
            {
                if (t.aimMode == 1)
                {
                    Vector3 target = GetThirdPersonTarget();
                    //Instantiate(turrets[0].muzzleFlash, target, Quaternion.Euler(Vector3.zero));
                    t.AimAt(target, false);
                }
            }
        }
        
        if (pressedKeys.Contains(KeyCode.LeftControl) || overrideAlt)
        {
            if (pressedKeys.Contains(KeyCode.Q) || overrideQ)
            {
                FireThrusters(q_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.y > speedTol)
                {
                    PID(relRot.y, speedTol, e_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.E) || overrideE)
            {
                FireThrusters(e_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.y < -speedTol)
                {
                    PID(relRot.y, -speedTol, q_thrusters, Kp, stabalizerPower);
                }
            }
            if ((pressedKeys.Contains(KeyCode.W)) || overrideW)
            {
                FireThrusters(alt_w_thrusters, maneuveringPower);
            } else
            {
                if (stabalizers && relVel.z > speedTol)
                {
                    PID(relVel.z, speedTol, alt_s_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.S) || overrideS)
            {
                FireThrusters(alt_s_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.z < -speedTol)
                {
                    PID(relVel.z, -speedTol, alt_w_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.A) || overrideA)
            {
                FireThrusters(alt_a_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.x < -speedTol)
                {
                    PID(relVel.x, -speedTol, alt_d_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.D) || overrideD)
            {
                FireThrusters(alt_d_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.x > speedTol)
                {
                    PID(relVel.x, speedTol, alt_a_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.Space) || overrideSpace)
            {
                FireThrusters(alt_space_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.y > speedTol)
                {
                    PID(relVel.y, speedTol, alt_shift_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.LeftShift) || overrideShift)
            {
                FireThrusters(alt_shift_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.y < -speedTol)
                {
                    PID(relVel.y, -speedTol, alt_space_thrusters, Kp, stabalizerPower);

                    if (useMainDriveForStabalization)
                        PID(relVel.y, -speedTol, space_thrusters, Kp, stabalizerPower);
                }
            }
        } else
        {
            if ((pressedKeys.Contains(KeyCode.W)) || overrideW)
            {
                FireThrusters(w_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.x > speedTol)
                {
                    PID(relRot.x, speedTol, s_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.S) || overrideS)
            {
                FireThrusters(s_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.x < -speedTol)
                {
                    PID(relRot.x, -speedTol, w_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.A) || overrideA)
            {
                FireThrusters(a_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.z > speedTol)
                {
                    PID(relRot.z, speedTol, d_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.D) || overrideD)
            {
                FireThrusters(d_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.z < -speedTol)
                {
                    PID(relRot.z, -speedTol, a_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.Space) || overrideSpace)
            {
                FireThrusters(space_thrusters, mainDrivePower);

                if (useStabalizersForMainThrust)
                {
                    FireThrusters(alt_space_thrusters, maneuveringPower);
                }
            }
            else
            {
                if (stabalizers && relVel.y > speedTol && !IsThrusting(space_thrusters))
                {
                    PID(relVel.y, speedTol, shift_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.LeftShift) || overrideShift)
            {
                FireThrusters(shift_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relVel.y < -speedTol && !IsThrusting(shift_thrusters))
                {
                    if (useMainDriveForStabalization)
                        PID(relVel.y, -speedTol, space_thrusters, Kp, stabalizerPower);
                    else
                        PID(relVel.y, -speedTol, alt_space_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.Q) || overrideQ)
            {
                FireThrusters(q_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.y > speedTol && !IsThrusting(q_thrusters))
                {
                    PID(relRot.y, speedTol, e_thrusters, Kp, stabalizerPower);
                }
            }
            if (pressedKeys.Contains(KeyCode.E) || overrideE)
            {
                FireThrusters(e_thrusters, maneuveringPower);
            }
            else
            {
                if (stabalizers && relRot.y < -speedTol && !IsThrusting(e_thrusters))
                {
                    PID(relRot.y, -speedTol, q_thrusters, Kp, stabalizerPower);
                }
            }
        }
        if (pointTo != null && (!movementKeysPressed || pressedKeys.Contains(KeyCode.LeftControl)))
        {
            Vector3 dir = (pointTo.transform.position - rb.position).normalized;
            if (Mathf.Abs(Vector3.Angle(dir, transform.up)) < angleTol)
            {
                StabalizeRotation(relRot, Kp);
            }
            else
            {
                TurnToPoint(dir, relRot);
            }
        }
        if (goToPoint != Vector3.zero && !movementKeysPressed)
        {
            Vector3 dir = (goToPoint - rb.position).normalized;
            float dist = Vector3.Distance(rb.position, goToPoint);

            if (goToPointOld != goToPoint)
            {
                goToPointOld = goToPoint;
                ogDist = dist;
                maxProjVel = 0;
                if (stabalizers)
                {
                    prev_stabalizers = true;
                    stabalizers = false;
                }
            }

            if (dist < speedTol*10)
            {
                if (relRot.magnitude > speedTol)
                    StabalizeRotation(relRot, Kp);
                else if (relVel.magnitude > speedTol)
                    StabalizePosition(relVel, Kp, false);
            }
            else
            {
                //if (dist < ogDist * 0.6f)
                float projVel = Vector3.Dot(relVelGlob, dir);
                if (projVel > maxProjVel)
                    maxProjVel = projVel;
                if (dist <= maxProjVel * flip_n_burn_time + 2*(ogDist-dist))
                {
                    flip_n_burn = true;
                }
                else
                {
                    flip_n_burn = false;
                    if (Mathf.Abs(Vector3.Angle(dir, transform.up)) < angleTol)
                    {
                        StabalizeRotation(relRot, Kp);
                        FireThrusters(space_thrusters, Mathf.Min(dist / 100f, 1));
                    } else
                    {
                        TurnToPoint(dir, relRot);

                        if (Mathf.Abs(Vector3.Angle(dir, transform.up)) < angleTol * 10)
                        {
                            StabalizePosition(transform.InverseTransformDirection(-dir*25), Kp, true);
                        }
                        else if (Mathf.Abs(Vector3.Angle(dir, transform.up)) < angleTol * 30)
                        {
                            StabalizePosition(transform.InverseTransformDirection(-dir*25), Kp, false);
                        }
                    }
                }

            }
        }
        if (flip_n_burn && !movementKeysPressed) // transform.InverseTransformDirection
        {
            if (stabalizers)
            {
                prev_stabalizers = true;
                stabalizers = false;
            }
            //TurnToPoint(-rb.velocity, relRot);

            if (relVel.magnitude < speedTol * 50)
            {
                if (relRot.magnitude > speedTol)
                    StabalizeRotation(relRot, Kp);
                else if (relVel.magnitude > speedTol)
                    StabalizePosition(relVel, Kp, false);
                else if (relVel.magnitude < speedTol)
                {
                    flip_n_burn = false;
                    stabalizers = prev_stabalizers;
                    prev_error_rot = -1;
                    rot_dir = 0;
                }
            }
            else if (Mathf.Abs(Vector3.Angle(-relVelGlob, transform.up)) < angleTol)
            {
                rot_dir = 0;
                StabalizeRotation(relRot, Kp);
                PID(relVel.y, -speedTol, space_thrusters);
                if (useStabalizersForMainThrust)
                    PID(relVel.y, -speedTol, alt_space_thrusters);
                PID(relVel.y, speedTol, shift_thrusters);
                
            }
            else
            {
                TurnToPoint(-relVelGlob, relRot);
                
                if (Mathf.Abs(Vector3.Angle(-relVelGlob, transform.up)) < angleTol*10)
                {
                    StabalizePosition(relVel, Kp, true);
                }
                else if (Mathf.Abs(Vector3.Angle(-relVelGlob, transform.up)) < angleTol * 30)
                {
                    StabalizePosition(relVel, Kp, false);
                }
                
            }
        }
    }

    public bool IsThrusting(Thruster[] thrusters)
    {
        if (thrusters.Length == 0) return false;
        bool result = false;
        foreach (Thruster t in thrusters)
        {
            if (t != null && t.enabled && t.gameObject.activeSelf)
            {
                if (!t.Thrusting())
                {
                    return false;
                }
                else
                {
                    result = true;
                }
            }
        }
        return result;
    }

    public bool CanSee(SpaceObject e)
    {
        foreach (Radar r in radars)
        {
            if (r != null && r.enabled && r.gameObject.activeSelf && r.RadarObject(e.gameObject, playerShip && player.traceRadar))
            {
                return true;
            }
        }
        return false;
    }

    public bool CanTarget(SpaceObject e)
    {
        foreach (Targeter r in targeters)
        {
            if (r != null && r.enabled && r.gameObject.activeSelf && r.TargetObject(e.gameObject, playerShip && player.traceRadar))
            {
                return true;
            }
        }
        return false;
    }

    public void Target(GameObject o, int weaponIndex, int weaponType) // weaponType 0 == turret, 1 == missile, 2 == cannon
    {
        if (reactor.telemetryPower)
        {
            foreach (Targeter t in targeters)
            {
                if (t.TargetObject(o, player.traceRadar) && playerShip)
                {
                    GameObject temp = MakeUITarget(o);
                    Indicator indi = temp.GetComponent<Indicator>();
                    if (weaponType == 0) // turret
                    {
                        if (weaponIndex == -1)
                        {
                            foreach (gun g in turrets)
                            {
                                if (!g.targets.ContainsKey(indi.target))
                                    g.targets.Add(indi.target, true);
                            }
                        } else 
                            if (!turrets[weaponIndex].targets.ContainsKey(indi.target))
                                turrets[weaponIndex].targets.Add(indi.target, true);
                    } else if (weaponType == 1) // missile launcher
                    {
                        if (weaponIndex == -1) {
                            foreach (MissileLauncher l in launchers)
                            {
                                l.Launch(indi.target, indi);
                            }
                        } else
                        {
                            launchers[weaponIndex].Launch(indi.target, indi);
                        }
                    } else if (weaponType == 2) // cannon
                    {
                        if (weaponIndex == -1)
                        {
                            foreach (gun g in statGuns)
                            {
                                if (!g.targets.ContainsKey(indi.target))
                                    g.targets.Add(indi.target, true);
                            }
                        }
                        else
                            if (!statGuns[weaponIndex].targets.ContainsKey(indi.target))
                            statGuns[weaponIndex].targets.Add(indi.target, true);
                    }
                    break;
                }
            }
        }
    }

    GameObject MakeUITarget(GameObject targetObject)
    {
        GameObject temp = Instantiate(player.circle, player.canvas.transform);
        temp.GetComponent<Image>().color = Color.red;
        temp.GetComponent<Image>().sprite = player.targetSprite;
        temp.transform.localEulerAngles = Vector3.zero;
        temp.GetComponent<Indicator>().cam = player.cam;
        temp.GetComponent<Indicator>().target = targetObject.gameObject;
        temp.GetComponent<Indicator>().dist = player.canvas.planeDistance;
        temp.GetComponent<Indicator>().trackObject = true;
        temp.GetComponentInChildren<TextMeshProUGUI>().text = targetObject.GetComponent<SpaceObject>().name;
        temp.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        temp.tag = "UI Target";
        return temp;
    }

    GameObject MakeLostUITarget(GameObject targetObject, Vector3 lastPos, Vector3 lastVel)
    {
        GameObject temp = Instantiate(player.circle, player.canvas.transform);
        Image sr = temp.GetComponent<Image>();
        Indicator indi = temp.GetComponent<Indicator>();
        TextMeshProUGUI tmp = temp.GetComponentInChildren<TextMeshProUGUI>();
        sr.color = Color.yellow;
        sr.sprite = player.targetSprite;
        temp.transform.localEulerAngles = Vector3.zero;
        indi.cam = player.cam;
        indi.target = targetObject.gameObject;
        indi.dist = player.canvas.planeDistance;
        indi.trackObject = true;
        tmp.text = targetObject.GetComponent<SpaceObject>().name;
        tmp.color = Color.yellow;
        temp.tag = "UI Lost Target";
        indi.guess = true;
        indi.guessPos = lastPos;
        indi.guessVel = lastVel;
        return temp;
    }

    void ConvertTargetLockIndicatorStatus(GameObject indiObj, bool status)
    {
        ConvertTargetLockIndicatorStatus(indiObj, status, Vector3.zero, Vector3.zero);
    }

    void ConvertTargetLockIndicatorStatus(GameObject indiObj, bool status, Vector3 lastPos, Vector3 lastVel)
    {
        Image sr = indiObj.GetComponent<Image>();
        Indicator indi = indiObj.GetComponent<Indicator>();
        TextMeshProUGUI tmp = indiObj.GetComponentInChildren<TextMeshProUGUI>();
        if (status)
        {
            sr.color = Color.red;
            tmp.color = Color.red;
            indi.guess = false;
            //indiObj.tag = "UI Target";
        } else
        {
            sr.color = Color.yellow;
            tmp.color = Color.yellow;
            indi.guess = true;
            indi.guessPos = lastPos;
            indi.guessVel = lastVel;
            //indiObj.tag = "UI Lost Target";
        }
    }

    private bool prev_stabalizers = false;
   
    public void TurnToPoint(Vector3 target, Vector3 relRot)
    {
        target = target.normalized;
        float w_angle = Vector3.SignedAngle(transform.up, Yeet.Perp(target, transform.right), transform.right);
        float d_angle = Vector3.SignedAngle(transform.up, Yeet.Perp(target, transform.forward), transform.forward);

        w_angle = -Yeet.FixAngle(w_angle);
        d_angle = -Yeet.FixAngle(d_angle);

        PID2(w_angle / 180f, relRot.x, w_thrusters, s_thrusters, pointToKp, pointToKd);
        PID2(d_angle / 180f, relRot.z, a_thrusters, d_thrusters, pointToKp, pointToKd);

        //Debug.Log("relRot=" + relRot);
    }

    public void TurnToPointOld2(Vector3 target, Vector3 relRot)
    {
        Vector3 axis = Vector3.Cross(target, transform.up).normalized;

        target = Quaternion.AngleAxis(-transform.eulerAngles.y, transform.up) * target;

        if (axis == Vector3.zero)
        {
            if (target.normalized == -transform.up.normalized)
                axis = new Vector3(1, 0, 0);
        }
        float angle = Vector3.SignedAngle(target, transform.up, axis);

        Vector3 targetRot = Yeet.FixAngle(angle * axis);
        //Debug.Log("Dir = " + transform.up + " target = " + target + " targetRot = " + targetRot);

        if (Vector3.Angle(target, transform.up) < 5)
        {
            rb.angularVelocity = Vector3.zero;
            target = target - transform.position;
            target = target.normalized;

            //target = Quaternion.AngleAxis(-transform.eulerAngles.y, transform.up) * target

            axis = Vector3.Cross(target, transform.up).normalized;
            if (axis == Vector3.zero)
            {
                if (target.normalized == -transform.up.normalized)
                    axis = transform.right;
            }
            angle = Vector3.SignedAngle(target, transform.up, axis);

            targetRot = Yeet.FixAngle(angle * axis);

            //targetRot = transform.TransformDirection

            //targetRot = Quaternion.AngleAxis(angle, axis) * transform.up;

            Debug.Log("Axis = " + axis + " angle = " + angle + " targetRot = " + targetRot);


            transform.eulerAngles -= targetRot;
            if (Mathf.Abs(Vector3.SignedAngle(target, transform.up, axis)) > Mathf.Abs(angle))
                transform.eulerAngles += 2 * targetRot;
            return;
        }

        PID2(targetRot.x/180f, relRot.x, w_thrusters, s_thrusters, 10, 5);
        //PID2(targetRot.y/180f, relRot.y, q_thrusters, e_thrusters, 10, 5);
        PID2(targetRot.z/180f, relRot.z, a_thrusters, d_thrusters, 10, 5);
    }

    public bool isEnemy(SpaceShip o)
    {
        if (o == this)
            return false;
        switch (team)
        {
            case 0:
                if (o.team == 1)
                    return true;
                break;
            case 1:
                if (o.team == 0)
                    return true;
                break;
        }
        return false;
    }
        
    public bool cancelTurn = false;
    int turnStage = 0;
    float w_effect = 0;
    float s_effect = 0;
    float a_effect = 0;
    float d_effect = 0;
    float max_effect = 0;
    float start_angle = 0;
    float prop = 0;

    private void TurnToPointOld(Vector3 target, Vector3 relRot)
    {
        target = target.normalized;
        //Debug.Log("Turn Stage = " + turnStage);
        Vector3 axis = Vector3.Cross(target, transform.up).normalized;
        float angle = Vector3.Angle(target, transform.up);

        if (turnStage == 0)
        {
            w_effect = 0;
            s_effect = 0;
            a_effect = 0;
            d_effect = 0;
            max_effect = 0;
            start_angle = 0;
            prop = 0;
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol && angle > angleTol * 5)
                turnStage = 1;
            //else if (angle < angleTol * 5)
            //    turnStage = 18;
        }
        else if (turnStage == 1)
        {
            prop = angle / 180f;
            start_angle = angle;
            if (Random.Range(0, 2) == 0)
                turnStage += 1;
            else
                turnStage += 3;
        }
        else if (turnStage == 2)
        {
            FireThrusters(w_thrusters, stabalizerPower * prop);
            if (angle < start_angle * 0.55f)
                turnStage += 3;
            else if (angle > start_angle + speedTol)
                turnStage += 1;
        }
        else if (turnStage == 3)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage += 1;
        }
        else if (turnStage == 4)
        {
            FireThrusters(s_thrusters, stabalizerPower * prop);
            if (angle < start_angle * 0.55f)
                turnStage += 1;
            else if (angle > start_angle + speedTol)
                turnStage -= 4;
        }
        else if (turnStage == 5)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
            {
                if (Random.Range(0, 2) == 0)
                    turnStage += 1;
                else
                    turnStage += 3;
            }
        }
        else if (turnStage == 6)
        {
            prop = angle / 180f;
            FireThrusters(a_thrusters, stabalizerPower * prop);
            if (angle < start_angle * 0.55f)
                turnStage += 3;
            else if (angle > start_angle + speedTol)
                turnStage += 1;
        }
        else if (turnStage == 7)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage += 1;
        }
        else if (turnStage == 8)
        {
            prop = angle / 180f;
            FireThrusters(d_thrusters, stabalizerPower * prop);
            if (angle < start_angle * 0.55f)
                turnStage += 1;
            else if (angle > start_angle + speedTol)
                turnStage -= 3;
        }
        else if (turnStage == 9)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage = 0;
        }
        else if (turnStage == 18)
        {
            prop = angle / 90f;
            start_angle = Vector3.Distance(target, transform.up);
            if (Random.Range(0, 2) == 0)
                turnStage += 1;
            else
                turnStage += 3;
        }
        else if (turnStage == 19)
        {
            prop = angle / 90f;
            FireThrusters(w_thrusters, stabalizerPower * prop);
            if (Vector3.Distance(target, transform.up) < start_angle * 0.55f)
                turnStage += 3;
            else if (Vector3.Distance(target, transform.up) > start_angle + speedTol)
                turnStage += 1;
        }
        else if (turnStage == 20)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage += 1;
        }
        else if (turnStage == 21)
        {
            prop = angle / 90f;
            FireThrusters(s_thrusters, stabalizerPower * prop);
            if (Vector3.Distance(target, transform.up) < start_angle * 0.55f)
                turnStage += 1;
            else if (Vector3.Distance(target, transform.up) > start_angle + speedTol)
                turnStage = 0;
        }
        else if (turnStage == 22)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
            {
                if (Random.Range(0, 2) == 0)
                    turnStage += 1;
                else
                    turnStage += 3;
            }
        }
        else if (turnStage == 23)
        {
            prop = angle / 90f;
            FireThrusters(a_thrusters, stabalizerPower * prop);
            if (Vector3.Distance(target, transform.up) < start_angle * 0.55f)
                turnStage += 3;
            else if (Vector3.Distance(target, transform.up) > start_angle + speedTol)
                turnStage += 1;
        }
        else if (turnStage == 24)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage += 1;
        }
        else if (turnStage == 25)
        {
            prop = angle / 90f;
            FireThrusters(d_thrusters, stabalizerPower * prop);
            if (Vector3.Distance(target, transform.up) < start_angle * 0.55f)
                turnStage += 1;
            else if (Vector3.Distance(target, transform.up) > start_angle + speedTol)
                turnStage -= 3;
        }
        else if (turnStage == 26)
        {
            StabalizeRotation(relRot, Kp);
            if (Mathf.Abs(relRot.magnitude) < speedTol)
                turnStage = 0;
        }
    }    

    public void FireThrusters(Thruster[] thrusters, float power)
    {
        if (power <= 0)
            return;
        if (Game.instance.record && c % 10 == 0)
        {
            Game.instance.rec.LogThrust(this, thrusters, power > 0, power);
        }
        foreach (Thruster t in thrusters)
            if (t)
                t.Thrust(power);
    }

    public void StopThrusters(Thruster[] thrusters)
    {
        if (Game.instance.record && c % 10 == 0)
        {
            Game.instance.rec.LogThrust(this, thrusters, false, 0);
        }
        foreach (Thruster t in thrusters)
            if (t)
                t.StopThrust();
    }

    private void StabalizeRotation(Vector3 relRot, float Kp)
    {
        float speedTol = this.speedTol * 0.5f;
        if (relRot.x < -speedTol)
        {
            PID(relRot.x, -speedTol, w_thrusters, Kp);
        }
        if (relRot.x > speedTol)
        {
            PID(relRot.x, speedTol, s_thrusters, Kp);
        }
        if (relRot.y > speedTol)
        {
            PID(relRot.y, speedTol, e_thrusters, Kp);
        }
        if (relRot.y < -speedTol)
        {
            PID(relRot.y, -speedTol, q_thrusters, Kp);
        }
        if (relRot.z > speedTol)
        {
            PID(relRot.z, speedTol, d_thrusters, Kp);
        }
        if (relRot.z < -speedTol)
        {
            PID(relRot.z, -speedTol, a_thrusters, Kp);
        }
    }

    private void StabalizePosition(Vector3 relVel, float Kp, bool useMainDrive)
    {
        if (relVel.z > speedTol)
        {
            PID(relVel.z, speedTol, alt_s_thrusters, Kp, stabalizerPower);
        }

        if (relVel.z < -speedTol)
        {
            PID(relVel.z, -speedTol, alt_w_thrusters, Kp, stabalizerPower);
        }

        if (relVel.x < -speedTol)
        {
            PID(relVel.x, -speedTol, alt_d_thrusters, Kp, stabalizerPower);
        }

        if (relVel.x > speedTol)
        {
            PID(relVel.x, speedTol, alt_a_thrusters, Kp, stabalizerPower);
        }

        if (relVel.y > speedTol)
        {
            PID(relVel.y, speedTol, alt_shift_thrusters, Kp, stabalizerPower);
        }

        if (relVel.y < -speedTol)
        {
            PID(relVel.y, -speedTol, alt_space_thrusters, Kp, stabalizerPower);
            if (useMainDrive)
                PID(relVel.y, -speedTol, space_thrusters, Kp, stabalizerPower);
        }
    }

    private float prev_error_rot = -1;
    private int rot_dir = 0;

    public float Kp = 1.5f;
    public float Kd = 1.5f;

    public float pointToKp = 8;
    public float pointToKd = 5;

    private void PID(float error, float errorTol, Thruster[] thrusters)
    {
        PID(error, errorTol, thrusters, this.Kp);
    }

    private void PID(float error, float errorTol, Thruster[] thrusters, float Kp)
    {
        error = error * Mathf.Sign(errorTol);

        if (Game.instance.record && c % 10 == 0)
        {
            Game.instance.rec.LogThrust(this, thrusters, Kp * error > 0, Kp*error);
        }

        foreach (Thruster t in thrusters)
        {
            if (t != null)
                t.Thrust(Kp * error);
        }
    }

    private void PID(float error, float errorTol, Thruster[] thrusters, float Kp, float max)
    {
        error = error * Mathf.Sign(errorTol);

        if (Game.instance != null && Game.instance.record && c % 10 == 0)
        {
            Game.instance.rec.LogThrust(this, thrusters, Mathf.Min(Kp * error, max) > 0, Mathf.Min(Kp*error, max));
        }

        foreach (Thruster t in thrusters)
        {
            if (t != null)
                t.Thrust(Mathf.Min(Kp * error, max));
        }
    }

    private void PID2(float error, float deltaError, Thruster[] pos, Thruster[] neg, float Kp, float Kd)
    {
        float result = Kp * error + Kd * deltaError;
        //Debug.Log("error = " + error + " deltaError = " + deltaError + " Result = " + result);

        if (Game.instance.record && c % 10 == 0)
        {
            Game.instance.rec.LogThrust(this, neg, result > 0, result);
            Game.instance.rec.LogThrust(this, pos, -result > 0, -result);
        }

        if (result > 0)
            foreach (Thruster t in neg)
                t.Thrust(result);
        else
            foreach (Thruster t in pos)
                t.Thrust(-result);
    }

    public int GetThrusterGroupNumber(Thruster[] thrusters)
    {
        if (thrusters == w_thrusters)
            return 0;
        if (thrusters == a_thrusters)
            return 1;
        if (thrusters == d_thrusters)
            return 2;
        if (thrusters == s_thrusters)
            return 3;
        if (thrusters == space_thrusters)
            return 4;
        if (thrusters == shift_thrusters)
            return 5;
        if (thrusters == q_thrusters)
            return 6;
        if (thrusters == e_thrusters)
            return 7;
        if (thrusters == alt_w_thrusters)
            return 8;
        if (thrusters == alt_s_thrusters)
            return 9;
        if (thrusters == alt_a_thrusters)
            return 10;
        if (thrusters == alt_d_thrusters)
            return 11;
        if (thrusters == alt_space_thrusters)
            return 12;
        if (thrusters == alt_shift_thrusters)
            return 13;
        return -1;
    }

    public Thruster[] GetThrustersFromID(int id)
    {
        switch (id)
        {
            case 0:
                return w_thrusters;
            case 1:
                return a_thrusters;
            case 2:
                return d_thrusters;
            case 3:
                return s_thrusters;
            case 4:
                return space_thrusters;
            case 5:
                return shift_thrusters;
            case 6:
                return q_thrusters;
            case 7:
                return e_thrusters;
            case 8:
                return alt_w_thrusters;
            case 9:
                return alt_s_thrusters;
            case 10:
                return alt_a_thrusters;
            case 11:
                return alt_d_thrusters;
            case 12:
                return alt_space_thrusters;
            case 13:
                return alt_shift_thrusters;
        }
        return null;
    }

    private void OnDestroy()
    {
        //thirdPersonCamera.transform.parent = null;
    }

}