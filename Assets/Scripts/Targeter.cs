using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : Radar
{

    //public GameObject point;
    //public int range = 100;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TargetObject(GameObject obj, bool trace)
    {
        bool radared = RadarObject(obj, trace);

        if (radared)
        {
            SpaceShip o_ship = obj.GetComponent<SpaceShip>();
            if (o_ship == null)
                o_ship = obj.GetComponentInParent<SpaceShip>();
            if (o_ship != null)
            {
                if (o_ship.jamTargetters)
                    return false;
                else
                    o_ship.targetted = true;
            }
            return true;
        } else
        {
            return false;
        }
    }
}
