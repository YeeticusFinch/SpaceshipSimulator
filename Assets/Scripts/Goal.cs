using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : SpaceObject
{
    public bool startTimer = false;
    public bool endTimer = false;
    public bool shooting = false;
    public float HP = 20;
    public bool position = true;
    public Vector3 distance = Vector3.zero;
    public Vector3 relPos = Vector3.zero;

    [System.NonSerialized]
    public bool done = false;
    [System.NonSerialized]
    public bool current = false;

    public Material distantMat;
    public Material nextMat;
    public Material currentMat;
    public Material doneMat;
    public int matIndex = 0;

    public Goal nextGoal;

    [System.NonSerialized]
    public bool isNext = false;

    MeshRenderer mr;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        if (GetComponentInParent<Game>() == null || GetComponentInParent<Game>().mapID != Game.mapNum)
        {
            gameObject.SetActive(false);
            return;
        }
        mr = GetComponent<MeshRenderer>();
        if (startTimer)
            current = true;
        if (shooting)
            transform.parent = null;
    }

    private void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        Debug.Log("YEET");
    }

    int c = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        c++;
        c %= 10000;

        if (c%4 == 0)
        {
            if (!done && !current)
            {
                if (isNext)
                    SetMaterial(nextMat);
                else
                    SetMaterial(distantMat);
            } else if (current)
            {
                if (nextGoal != null)
                    nextGoal.isNext = true;
                SetMaterial(currentMat);
            } else if (done)
            {
                SetMaterial(doneMat);
            }
        }

        if (current && Game.instance != null && Game.instance.player != null && Game.instance.player.ship != null)
        {
            if (position)
            {
                Vector3 diff = transform.InverseTransformDirection(Game.instance.player.ship.transform.position - transform.position);
                //Debug.Log("Diff = " + diff + ", Dist = " + distance);
                if (Mathf.Abs(diff.x) < distance.x && Mathf.Abs(diff.y) < distance.y && Mathf.Abs(diff.z) < distance.z)
                {
                    GoalComplete();
                }
            } else if (shooting)
            {
                if (HP < 0.1f)
                {
                    GoalComplete();
                    Game.instance.player.ship.ClearTargetLock(gameObject);
                    if (GetComponent<SimpleMotionObject>() != null)
                        Destroy(GetComponent<SimpleMotionObject>());
                }
            }
        }
    }

    void GoalComplete()
    {
        if (startTimer)
            Game.instance.StartTimer();
        else if (endTimer)
            Game.instance.StopTimer();
        //Debug.Log("Goal Complete!!!");
        current = false;
        done = true;
        if (nextGoal != null)
            nextGoal.current = true;
    }

    void SetMaterial(Material mat)
    {
        Material[] mats = mr.materials;
        mats[matIndex] = mat;
        mr.materials = mats;
    }
}
