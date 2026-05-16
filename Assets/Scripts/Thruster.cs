using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ShipObject
{
    public bool snapThrust = false;
    public bool useThrustPos = false;
    public GameObject thrustPos;
    public Servo[] servos;
    public bool servoDisable = false;
    public bool fake = false;
    public bool needsReactor = false;
    public bool copyThrusterGroup = true;
    public bool mainDrive = false;
    public bool teaKettle = false;
    public bool misc = false;
    public bool misc2 = false;

    public float power = 1;
    public float thrustOffsetY = -0.5f;
    public GameObject thrust;

    public float exhaustVelocityFactor = 0.4f;
    public int exhaustLife = 5;
    public float exhaustScaleFactor = 1f;
    public bool parentThrustToThruster = false;
    public float lightIntensityFactor = 1f;
    public float maxLightIntensity = 15;
    public float smokerOffset = -0.0357f;
    public float smokerScale = 1;
    public float batteryConsumption = 0.02f;
    //public int smokerSpeed = 20;

    public Light thrusterLight;

    public MomentumWheelAssembly wheelAssembly;
    public int dirNum;

    float powerMult = 1;

    //Rigidbody rb;

    //GameObject flame;

    private static float thrustFactor = 0.02f;

    ParticleSystem smoker;

    public SoundManager.Sound thrustSound;
    bool playSounds = false;
    float maxVol = 0;

    public void GrabValues(Thruster other)
    {
        power = other.power;
        thrustOffsetY = other.thrustOffsetY;
        thrust = other.thrust;
        parentThrustToThruster = other.parentThrustToThruster;
        exhaustVelocityFactor = other.exhaustVelocityFactor;
        exhaustLife = other.exhaustLife;
        exhaustScaleFactor = other.exhaustScaleFactor;
        maxHP = other.maxHP;
        HP = other.HP;
        maxRotation = other.maxRotation;
        dmgFactor = other.dmgFactor;
    }


    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        maxVol = thrustSound.vol;
        if (!fake && Game.instance != null && Game.instance.atmosphere)
        {
            smoker = Instantiate(Resources.Load("Smoke Particle") as GameObject).GetComponent<ParticleSystem>();
            smoker.transform.parent = transform;
            smoker.transform.localPosition = new Vector3(0, smokerOffset, 0);
            smoker.transform.localEulerAngles = new Vector3(90, 0, 0);
            smoker.transform.localScale = smokerScale * Vector3.one;
            smoker.Pause();
            if (!mainDrive)
            {
                smoker.maxParticles = 100;
                smoker.emissionRate = 3;
            }
        }
        if (!forMissile)
        {
            if (ship == null)
                ship = GetComponentInParent<SpaceShip>();
            if (reactor == null)
                reactor = ship.reactor;
            rb = ship.GetComponent<Rigidbody>();
            playSounds = ship.playSounds;
        } else
        {
            rb = GetComponentInParent<Rigidbody>();
        }
        //flame = Resources.Load("Thrust") as GameObject;
    }

    [System.NonSerialized]
    public float thrustAmount = 0;
    private float thrustThreshold = 0.001f;

    int c = 0;

    public bool Thrusting()
    {
        
        if (servos != null && servos.Length > 0)
        {
            foreach (Servo s in servos)
            {
                if (!s.atPosition)
                {
                    //Debug.Log("Servo " + s.gameObject.name + " not at position");
                    return false;
                }
            }
            //Debug.Log("All servos at position");
        }
        
        //if (servoDisable)
        //    return false;
        return (forMissile || (reactor.drivePower && (reactor.power || (reactor.batteryPower && !needsReactor)))) && thrustAmount > 0.001f;
    }

    bool showThrust = false;

    private void FixedUpdate()
    {
        base.FixedUpdate();
        if (!fake && Game.instance != null && Game.instance.atmosphere && smoker == null)
        {
            smoker = Instantiate(Resources.Load("Smoke Particle") as GameObject).GetComponent<ParticleSystem>();
            smoker.transform.parent = transform;
            smoker.transform.localPosition = new Vector3(0, smokerOffset, 0);
            smoker.transform.localEulerAngles = new Vector3(90, 0, 0);
            smoker.transform.localScale = smokerScale * Vector3.one;
            smoker.Pause();
            if (!mainDrive)
            {
                smoker.maxParticles = 100;
                smoker.emissionRate = 3;
            }
        }
        c++;
        c %= 10000;
        if (c % 10 == 0)
        {
            if (!playSounds && (ship != null && ship.playSounds)) playSounds = true;
            if (!mainDrive && Vector2.Distance(Camera.main.transform.position, transform.position) > Yeet.particleRenderDistance) showThrust = false;
            else showThrust = true;
        }
        if (!forMissile && reactor == null)
        {
            if (thrusterLight != null)
                thrusterLight.intensity = 0;
            Debug.Log("No Reactor");
            return;
        }
        if (Thrusting())
        {
            PerformThrust();
            if (!fake && smoker != null && Game.instance.atmosphere)
            {
                if (!smoker.isPlaying)
                {
                    smoker.Play();
                }
                if (!smoker.loop)
                    smoker.loop = true;
                //smoker.transform.localScale = smokerScale * Mathf.Clamp(thrustAmount, 0, 1) * Vector3.one;
                smoker.startSize = 60 * Mathf.Clamp(thrustAmount, 0, 1);
            }
        } else if (!fake && Game.instance != null && Game.instance.atmosphere && smoker.loop)
        {
            smoker.loop = false;
            //smoker.Pause();
        }
        if (c % 10 == 0 && !forMissile)
        {
            powerMult = reactor.power ? (reactor.highPower ? 1.3f : (reactor.lowPower ? 0.5f : 1)) : 1;
        }
        if (c % 2 == 0 && !fake && thrusterLight != null)
        {
            if (forMissile || (reactor.drivePower && (reactor.power || (reactor.batteryPower && !needsReactor))))
            {
                float powerFormula = Mathf.Clamp(power * thrustAmount * thrustFactor * powerMult * lightIntensityFactor, 0, maxLightIntensity);
                if (thrusterLight.intensity < powerFormula)
                {
                    thrusterLight.intensity += 0.02f * powerFormula;
                    thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity, 0, maxLightIntensity);
                }
                else if (thrusterLight.intensity > powerFormula)
                    thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity - 2, 0, maxLightIntensity);
                //else
                //    thrusterLight.intensity = 0;
            }
            else
            {
                thrusterLight.intensity = 0;
            }
        }
        if (thrustAmount > 0 && wheelAssembly != null)
        {
            wheelAssembly.thrustAmounts[dirNum] = thrustAmount;
        }
        if (!(Game.instance != null && Game.instance.playbackRecording))
            thrustAmount = 0;
    }

    public void StopThrust()
    {
        thrustAmount = 0;
    }

    public void DisplayThrust(float amount)
    {
        if (showThrust)
        {
            GameObject exhaust = Instantiate(thrust) as GameObject;
            exhaust.GetComponent<Particle>().lifetime = exhaustLife;
            if (power * powerMult > 600) exhaust.tag = "BigThrust";
            else exhaust.tag = "Thrust";
            if (parentThrustToThruster)
            {
                exhaust.GetComponent<Particle>().localVelocity = -Vector3.up * power * powerMult * thrustFactor * amount * exhaustVelocityFactor;
                exhaust.transform.parent = transform;
                exhaust.transform.localPosition = Vector3.up * thrustOffsetY;
                //exhaust.GetComponent<Particle>().parent = true;
            }
            else
            {
                exhaust.GetComponent<Particle>().velocity = -transform.up * power * powerMult * thrustFactor * amount * exhaustVelocityFactor;
                exhaust.transform.position = transform.position + transform.up * thrustOffsetY * Time.fixedDeltaTime;
            }
            exhaust.transform.localScale *= power * powerMult * exhaustScaleFactor * amount * thrustFactor;
        }

        float powerFormula = Mathf.Clamp(power * amount * thrustFactor * powerMult * lightIntensityFactor, 0, maxLightIntensity);
        if (thrusterLight.intensity < powerFormula)
        {
            thrusterLight.intensity += 0.02f * powerFormula;
            thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity, 0, maxLightIntensity);
        }
        else if (thrusterLight.intensity > powerFormula)
            thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity - 2, 0, maxLightIntensity);
    }

    private Vector3 closestDir(Vector3 dir)
    {
        dir = dir.normalized;
        Vector3 result = ship.transform.InverseTransformDirection(dir).normalized;
        if (Mathf.Abs(result.x) > 0.8f && Mathf.Abs(result.y) < 0.2f && Mathf.Abs(result.z) < 0.2f)
            return ship.transform.TransformDirection(new Vector3(Mathf.Sign(result.x), 0, 0));
        if (Mathf.Abs(result.y) > 0.8f && Mathf.Abs(result.x) < 0.2f && Mathf.Abs(result.z) < 0.2f)
            return ship.transform.TransformDirection(new Vector3(0, Mathf.Sign(result.y), 0));
        if (Mathf.Abs(result.z) > 0.8f && Mathf.Abs(result.y) < 0.2f && Mathf.Abs(result.x) < 0.2f)
            return ship.transform.TransformDirection(new Vector3(0, 0, Mathf.Sign(result.z)));
        return Vector3.zero;
    }

    private void PerformThrust()
    {
        float amount = Mathf.Clamp(thrustAmount, 0, 1);

        if (!forMissile)
        {
            if (!reactor.power && !forMissile) reactor.batteryAmount -= batteryConsumption * amount;
        }

        if (fake) return;


        Vector3 thrustDir = transform.up;

        if (snapThrust)
        {
            Vector3 snapDir = closestDir(thrustDir);
            if (snapDir.magnitude > 0.1f)
            {
                thrustDir = snapDir;
            }
        }

        if (!(Game.instance != null && Game.instance.playbackRecording))
            rb.AddForceAtPosition(thrustDir * power * powerMult * thrustFactor * amount, useThrustPos && thrustPos != null ? thrustPos.transform.position : transform.position, ForceMode.Impulse);
        if (showThrust)
        {
            GameObject exhaust = Instantiate(thrust) as GameObject;
            exhaust.GetComponent<Particle>().lifetime = exhaustLife;
            if (power * powerMult > 600) exhaust.tag = "BigThrust";
            else exhaust.tag = "Thrust";
            if (parentThrustToThruster)
            {
                exhaust.GetComponent<Particle>().localVelocity = -Vector3.up * power * powerMult * thrustFactor * amount * exhaustVelocityFactor;
                exhaust.transform.parent = transform;
                exhaust.transform.localPosition = Vector3.up * thrustOffsetY;
                //exhaust.GetComponent<Particle>().parent = true;
            }
            else
            {
                exhaust.GetComponent<Particle>().velocity = -transform.up * power * powerMult * thrustFactor * amount * exhaustVelocityFactor + rb.velocity;
                exhaust.transform.position = transform.position + transform.up * thrustOffsetY + rb.velocity * Time.fixedDeltaTime;
            }
            exhaust.transform.localScale *= power * powerMult * exhaustScaleFactor * amount * thrustFactor;

        }
        if (playSounds)
        {
            thrustSound.vol = maxVol * amount * powerMult;
            thrustSound.play(transform.position, gameObject);
        }
    }

    public void Thrust(float amount)
    {
        thrustAmount += amount;
    }

    public void Thrust()
    {
        Thrust(1);
    }
}
