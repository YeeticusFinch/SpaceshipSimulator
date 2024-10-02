using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMotionObject : SpaceObject
{
    public Vector3 velocity;
    public Vector3 rotationalVelocity;
    public Vector3 force;
    public Vector3 torque;
    public bool alwaysTarget = false;
    public bool useMissileCollider = true;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        gameObject.tag = "AlwaysTarget";
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
        if (velocity.magnitude > 0.0001f)
            rb.velocity = transform.TransformDirection(velocity);
        if (rotationalVelocity.magnitude > 0.0001f)
            rb.angularVelocity = rotationalVelocity;
        if (force.magnitude > 0.0001f)
            rb.AddForce(transform.TransformDirection(force));
        if (torque.magnitude > 0.0001f)
            rb.AddTorque(torque);
    }
}
