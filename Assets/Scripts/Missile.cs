using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Missile : SpaceObject
{
    public GameObject camOrbit;
    public GameObject camSpot;

    public bool momentumTurn = false;
    public float turnSpeed = 2;
    public bool intercept = false;

    public Material velocityLine;
    public Material accelerationLine;

    [NonSerialized]
    public GameObject cam;
    [NonSerialized]
    public GameObject ogCamSpot;

    [NonSerialized]
    public Vector3 camTrans;
    [NonSerialized]
    public Vector3 camRot;

    //[NonSerialized]
    public SpaceShip owner;
    [NonSerialized]
    public Indicator targetIndi;
    [NonSerialized]
    public bool trace = false;

    public float leadFac = 0.35f;

    public GameObject target;

    bool checkedForRB;
    [NonSerialized]
    public bool chasing = false;
    Rigidbody targetRB;
    //[NonSerialized]
    //public Rigidbody rb;

    Vector3 targetPos;
    Vector3 targetRot;
    Vector3 targetVel;

    public Explosion explosion;
    public float explodeRange = 5;

    public Targeter[] targeters;

    [NonSerialized]
    public Vector3 safePos;
    [NonSerialized]
    public Vector3 safeVel;

    [NonSerialized]
    public bool teakettle = false;

    [NonSerialized]
    public bool manualControl = false;

    //[NonSerialized]
    public bool armed = false;

    //[NonSerialized]
    public bool matchCourse = false;

    [NonSerialized]
    public Vector3 safeDir;

    [NonSerialized]
    public bool useCamera = false;

    public bool canMove = true;

    public ShipObject[] hull;

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

    float speedTol = 5;
    float angleTol = 2;

    public int grace = 5;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        //rb = GetComponent<Rigidbody>();
        camOrbit.transform.parent = null;

        foreach (Collider col in GetComponentsInChildren<Collider>())
        {
            col.enabled = false;
        }
    }

    public void Explode()
    {
        GameObject g = Instantiate(explosion.gameObject, transform.position, Quaternion.Euler(Vector3.zero)) as GameObject;
        Explosion e = g.GetComponent<Explosion>();

        if (target.GetComponent<Rigidbody>() != null)
        {
            e.vel = rb.velocity + target.GetComponent<Rigidbody>().velocity / 2;
        } else
            e.vel = rb.velocity/2;
        g.transform.position = this.transform.position;
        Destroy(this.gameObject);
    }

    Vector3 velDiff;
    Vector3 relRot;
    Vector3 localTarget;

    int c = 0;

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
        c++;
        c %= 10000;
        if (useCamera)
        {
            camOrbit.transform.eulerAngles = Quaternion.LookRotation(target.transform.position - rb.position, Vector3.up).eulerAngles + camRot;
            camOrbit.transform.position = rb.position + camTrans;
        }

        if (c % 10 == 0)
        {
            if (grace > -1) grace--;
            if (grace == 0)
            {
                foreach (Collider col in GetComponentsInChildren<Collider>())
                {
                    col.enabled = true;
                }
            }
        }
        if (Game.instance.playbackRecording)
            return;

        if (rb.velocity.magnitude > (intercept ? Game.instance.maxInterceptMissileSpeed : Game.instance.maxMissileSpeed))
            rb.velocity = rb.velocity.normalized * (intercept ? Game.instance.maxInterceptMissileSpeed : Game.instance.maxMissileSpeed);
        float HP = 0;
        foreach (ShipObject o in hull)
        {
            HP += o.GetHP();
        }
        if (HP <= 0)
        {
            //Explode();
            Destroy(this.gameObject);
            return;
        }

        relRot = transform.InverseTransformDirection(rb.angularVelocity);

        if (!armed && safePos != null && safeVel != null && safePos != Vector3.zero)
        {
            targetVel = safeVel;
            velDiff = targetVel - rb.velocity;
            targetPos = safePos;
            localTarget = transform.InverseTransformPoint(targetPos);

            if (!armed && (Vector3.Distance(safePos, transform.position) < 1f || Vector3.Distance(rb.position, owner.rb.position) > 40))
            {
                armed = true;
                matchCourse = false;
                teakettle = false;
            }
            safePos += safeVel * Time.fixedDeltaTime;
        }
        else
        {
            if (target == null) return;
            if (targetRB == null && !checkedForRB)
            {
                checkedForRB = true;
                targetRB = target.GetComponent<Rigidbody>();
            }
            if (targetRB != null)
                targetVel = targetRB.velocity;
            else
                targetVel = Vector3.zero;

            velDiff = targetVel - rb.velocity;

            if (SeeTarget())
            {
                teakettle = false;
                //Debug.Log("Can See Target");
                if (Yeet.Perp(velDiff, target.transform.position-rb.position).magnitude > 20)
                //if (velDiff.magnitude > 20 && Vector3.Angle(transform.position - target.transform.position, velDiff) > 10)
                    matchCourse = true;

                if (matchCourse)
                {
                    targetPos = transform.position + velDiff;
                    localTarget = transform.InverseTransformPoint(targetPos);
                    if (velDiff.magnitude < 20)
                        matchCourse = false;
                }
                else
                {
                    targetPos = target.transform.position + velDiff;
                    localTarget = transform.InverseTransformPoint(targetPos);
                }
            }
            else // Move sideways until the missile can see the target
            {
                //Debug.Log("Target where are you?");
                teakettle = true;
                Vector3 diff = target.transform.position - transform.position;
                targetPos = transform.position + diff - Vector3.Project(diff, safeDir);
                localTarget = transform.InverseTransformPoint(targetPos).normalized * 100;
            }

            if (armed && Vector3.Distance(target.transform.position, transform.position) < explodeRange)
            {
                Explode();
                return;
            }
        }
        if (Mathf.Abs(relRot.y) > 0.1f)
        {
            StabalizeRotation(relRot, 3);
        }

        if (trace)
        {
            //drawLine(0, transform.position, transform.TransformPoint(localTarget));
            drawLine(1, rb.position, rb.position + rb.velocity, velocityLine);
            //drawLine(2, rb.position, rb.position + 100*rb.GetAccumulatedForce()/rb.mass, accelerationLine);
            //drawLine(3, rb.position + rb.velocity, rb.position + rb.velocity + 100*rb.GetAccumulatedForce() / rb.mass, accelerationLine);
        }

        if (teakettle)
        {
            FireThrusters(alt_w_thrusters, localTarget.normalized.z);
            FireThrusters(alt_s_thrusters, -localTarget.normalized.z);

            FireThrusters(alt_d_thrusters, localTarget.normalized.x);
            FireThrusters(alt_a_thrusters, -localTarget.normalized.x);

            FireThrusters(alt_space_thrusters, localTarget.normalized.y*0.1f);
            FireThrusters(alt_shift_thrusters, -localTarget.normalized.y);
        } else
        {
            Vector3 posMod = Vector3.zero;
            if (targetRB != null)
                posMod = targetRB.velocity * Time.fixedDeltaTime * Vector3.Distance(rb.position, targetRB.position) * leadFac;
            //posMod = Vector3.zero;
            //Debug.Log(name + " posmod=" + posMod);
            TurnToPoint(targetPos + posMod, relRot);
            if (Vector3.Angle(targetPos + posMod - transform.position, transform.up) < angleTol*5)
            {
                chasing = true;
                FireThrusters(space_thrusters, 1);
                StabalizeRotation(relRot, 3);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.collider.GetComponentInParent<SpaceShip>() != null && collision.collider.GetComponentInParent<SpaceShip>() == owner) || Vector3.Distance(owner.rb.position, rb.position) < explodeRange)
            return;
        else if (armed)
        {
            Explode();
        } else
        {
            base.OnCollisionEnter(collision);
        }
    }

    bool SeeTarget()
    {
        foreach (Targeter t in targeters)
        {
            if (t != null && target != null && t.TargetObject(target, false))
            {
                return true;
            }
        }
        return false;
    }

    private void TurnToPointNew(Vector3 target, Vector3 relRot)
    {
        target = target - transform.position;
        target = target.normalized;

        //target = Quaternion.AngleAxis(-transform.eulerAngles.y, transform.up) * target;

        if (Vector3.Angle(target, transform.up) < 1)
        {
            rb.angularVelocity = Vector3.zero;
            return;
        }

        Vector3 axis = Vector3.Cross(target, transform.up).normalized;
        if (axis == Vector3.zero)
        {
            if (target.normalized == -transform.up.normalized)
                axis = transform.right;
        }
        float angle = Vector3.Angle(target, transform.up);

        Vector3 targetRot = Yeet.FixAngle(angle * axis);
        //Debug.Log("Dir = " + transform.up + " target = " + target + " targetRot = " + targetRot);

        if (angle < 10)
        {
            rb.angularVelocity = Vector3.zero;
            transform.eulerAngles -= targetRot;
            return;
        }
        
        PID2(targetRot.x / 180f, relRot.x, w_thrusters, s_thrusters, 10, 2);
        PID2(targetRot.y / 180f, relRot.y, q_thrusters, e_thrusters, 10, 2);
        PID2(targetRot.z / 180f, relRot.z, a_thrusters, d_thrusters, 10, 2);
    }

    public void TurnToPoint(Vector3 target, Vector3 relRot)
    {
        target = target - transform.position;
        target = target.normalized;
        float w_angle = Vector3.SignedAngle(transform.up, Yeet.Perp(target, transform.right), transform.right);
        float d_angle = Vector3.SignedAngle(transform.up, Yeet.Perp(target, transform.forward), transform.forward);

        w_angle = -Yeet.FixAngle(w_angle);
        d_angle = -Yeet.FixAngle(d_angle);

        if (momentumTurn)
        {
            Vector3 turn = new Vector3(-w_angle, 0, -d_angle);
            MomentumRotate(turn.normalized * Mathf.Min(turnSpeed, turn.magnitude));
            return;
        }

        PID2(w_angle / 180f, relRot.x, w_thrusters, s_thrusters, 10, 2);
        PID2(d_angle / 180f, relRot.z, a_thrusters, d_thrusters, 10, 2);

        //Debug.Log("relRot=" + relRot);
    }

    private void TurnToPointOld2(Vector3 target, Vector3 relRot)
    {
        Vector3 targetLocal = transform.InverseTransformPoint(target);
        //Debug.Log(target);
        targetLocal = targetLocal.normalized;
        if (Vector3.Angle(targetLocal, Vector3.up) < 10)
        {
            rb.angularVelocity = Vector3.zero;
            target = target - transform.position;
            target = target.normalized;

            //target = Quaternion.AngleAxis(-transform.eulerAngles.y, transform.up) * target

            Vector3 axis = Vector3.Cross(target, transform.up).normalized;
            if (axis == Vector3.zero)
            {
                if (target.normalized == -transform.up.normalized)
                    axis = transform.right;
            }
            float angle = Vector3.Angle(target, transform.up);

            Vector3 targetRot = Yeet.FixAngle(angle * axis);

            //targetRot = transform.TransformDirection

            //targetRot = Quaternion.AngleAxis(angle, axis) * transform.up;

            //Debug.Log("Axis = " + axis + " angle = " + angle + " targetRot = " + targetRot);

            
            transform.eulerAngles -= targetRot;
            if (Vector3.Angle(target, transform.up) > angle)
                transform.eulerAngles += 2 * targetRot;
            rb.angularVelocity = Vector3.zero;
            return;
        }
        PID2(targetLocal.x, relRot.z, a_thrusters, d_thrusters, 10, 3);
        PID2(-targetLocal.z, relRot.x, w_thrusters, s_thrusters, 10, 3);
    }

    public void MomentumRotate(Vector3 rot)
    {
        transform.Rotate(Vector3.right, rot.x);
        transform.Rotate(Vector3.up, rot.y);
        transform.Rotate(Vector3.forward, rot.z);
    }

    public void FireThrusters(Thruster[] thrusters, float power)
    {
        if (!canMove) return;
        if (Game.instance.record)
        {
            Game.instance.rec.LogThrust(this, thrusters, power > 0, power);
        }
        if (power <= 0)
            return;
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
        if (momentumTurn)
        {
            rb.angularVelocity -= rb.angularVelocity.normalized * Mathf.Min(turnSpeed, rb.angularVelocity.magnitude);
            return;
        }
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
            PID(relVel.z, speedTol, alt_s_thrusters, Kp, 1);
        }

        if (relVel.z < -speedTol)
        {
            PID(relVel.z, -speedTol, alt_w_thrusters, Kp, 1);
        }

        if (relVel.x < -speedTol)
        {
            PID(relVel.x, -speedTol, alt_d_thrusters, Kp, 1);
        }

        if (relVel.x > speedTol)
        {
            PID(relVel.x, speedTol, alt_a_thrusters, Kp, 1);
        }

        if (relVel.y > speedTol)
        {
            PID(relVel.y, speedTol, alt_shift_thrusters, Kp, 1);
        }

        if (relVel.y < -speedTol)
        {
            PID(relVel.y, -speedTol, alt_space_thrusters, Kp, 1);
            if (useMainDrive)
                PID(relVel.y, -speedTol, space_thrusters, Kp, 1);
        }
    }

    private float prev_error_rot = -1;
    private int rot_dir = 0;

    public float Kp = 1.5f;

    private void PID(float error, float errorTol, Thruster[] thrusters)
    {
        PID(error, errorTol, thrusters, this.Kp);
    }

    private void PID(float error, float errorTol, Thruster[] thrusters, float Kp)
    {
        if (!canMove) return;
        error = error * Mathf.Sign(errorTol);
        if (Game.instance.record)
        {
            Game.instance.rec.LogThrust(this, thrusters, Kp * error > 0, Kp * error);
        }
        foreach (Thruster t in thrusters)
        {
            if (t != null)
                t.Thrust(Kp * error);
        }
    }

    private void PID(float error, float errorTol, Thruster[] thrusters, float Kp, float max)
    {
        if (!canMove) return;
        error = error * Mathf.Sign(errorTol);

        if (Game.instance.record)
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
        if (!canMove) return;
        float result = Kp * error + Kd * deltaError;

        if (Game.instance.record)
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

    private void OnDestroy()
    {
        if (Game.instance.record)
        {
            Game.instance.rec.KillObject(this);
        }
        if (useCamera)
        {
            cam.transform.parent = ogCamSpot.transform;
            cam.transform.localPosition = Vector3.zero;
            cam.transform.localEulerAngles = Vector3.zero;
            //StartCoroutine(DestroyIn(camOrbit, 1));
        }
        //Debug.Log("Missile Destroyed");
        if (targetIndi != null && targetIndi.gameObject != null)
        {
            //Debug.Log("Indicator Destroyed");
            Destroy(targetIndi.gameObject);
        }
        if (lineRenderer != null)
        {
            foreach (LineRenderer lr in lineRenderer)
                if (lr != null)
                    Destroy(lr.gameObject);
        }

        Destroy(camOrbit);
    }

    IEnumerator DestroyIn(GameObject dest, float delay)
    {
        yield return new WaitForSeconds(delay);
        GameObject.Destroy(dest);
    }

    LineRenderer[] lineRenderer = null;
    void drawLine(int id, Vector3 a, Vector3 b, Material material = null)
    {
        if (lineRenderer == null) lineRenderer = new LineRenderer[4];
        if (material == null)
            material = targeters[0].point.GetComponent<MeshRenderer>().material;
        //Debug.Log("Drawing line");
        if (lineRenderer[id] == null)
            lineRenderer[id] = new GameObject("MissileLine").AddComponent<LineRenderer>();
        if (GetComponent<Targeter>() != null)
        {
            lineRenderer[id].startColor = Color.red;
            lineRenderer[id].endColor = Color.red;
        }
        else
        {
            lineRenderer[id].startColor = Color.green;
            lineRenderer[id].endColor = Color.green;
        }
        lineRenderer[id].startWidth = 0.01f;
        lineRenderer[id].endWidth = 0.01f;
        lineRenderer[id].positionCount = 2;
        lineRenderer[id].useWorldSpace = true;

        Material[] mat = new Material[] { material };
        lineRenderer[id].materials = mat;
        lineRenderer[id].material = material;

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer[id].SetPosition(0, a); //x,y and z position of the starting point of the line
        lineRenderer[id].SetPosition(1, b); //x,y and z position of the end point of the line
        lineRenderer[id].receiveShadows = false;
        lineRenderer[id].shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        lineRenderer[id].generateLightingData = false;
        //StartCoroutine(FadeLine(lineRenderer));
    }

    /*
    IEnumerator FadeLine(LineRenderer l)
    {
        float fadeDuration = 0.5f;
        float fadePeriod = 0.1f;
        for (float i = 0; i < fadeDuration / fadePeriod; i += fadePeriod)
        {
            l.startWidth *= 0.5f;
            l.endWidth *= 0.5f;
            yield return new WaitForSeconds(fadePeriod);
        }
        Destroy(l.gameObject);

    }*/


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
}
