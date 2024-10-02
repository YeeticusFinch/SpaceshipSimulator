using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cockpit : ShipObject
{

    public int type = 0;
    public GameObject xAxisRot;
    public GameObject yAxisRot;
    public GameObject zAxisRot;

    public Vector3 acceleration;
    public Vector3 velocity;

    public Human[] people;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int c = 0;

    private void FixedUpdate()
    {
        base.FixedUpdate();
        c++;
        c %= 1000;
        if (c % 15 == 0 && ship != null) {
            Vector3 new_velocity = ship.rb.velocity + Vector3.Cross(ship.rb.angularVelocity, (transform.localPosition-ship.rb.centerOfMass));
            if (velocity != null)
            {
                acceleration = (new_velocity - velocity) / (15 * Time.fixedDeltaTime);
            }
            velocity = new_velocity;
        }
    }
}
