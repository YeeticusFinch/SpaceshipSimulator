using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thruster : ShipObject
{
    public bool fake = false;
    public bool needsReactor = false;
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
        return (forMissile || (reactor.drivePower && (reactor.power || (reactor.batteryPower && !needsReactor)))) && thrustAmount > 0.001f;
    }

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
        if (!playSounds && c % 10 == 0 && (ship != null && ship.playSounds)) playSounds = true;
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


        float powerFormula = Mathf.Clamp(power * amount * thrustFactor * powerMult * lightIntensityFactor, 0, maxLightIntensity);
        if (thrusterLight.intensity < powerFormula)
        {
            thrusterLight.intensity += 0.02f * powerFormula;
            thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity, 0, maxLightIntensity);
        }
        else if (thrusterLight.intensity > powerFormula)
            thrusterLight.intensity = Mathf.Clamp(thrusterLight.intensity - 2, 0, maxLightIntensity);
    }

    private void PerformThrust()
    {
        float amount = Mathf.Clamp(thrustAmount, 0, 1);

        if (!forMissile)
        {
            if (!reactor.power && !forMissile) reactor.batteryAmount -= batteryConsumption * amount;
        }

        if (fake) return;

        if (!(Game.instance != null && Game.instance.playbackRecording))
            rb.AddForceAtPosition(transform.up * power * powerMult * thrustFactor * amount, transform.position, ForceMode.Impulse);
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
