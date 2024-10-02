using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumWheel : ShipObject
{
    public float mass;
    public Vector3 axis;

    [System.NonSerialized]
    public float actualMass;

    //[System.NonSerialized]
    public float spinSpeed = 0f;

    float maxSpinSpeed = 50;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        actualMass = mass;
        foreach (MomentumWheel mw in gameObject.GetComponentsInChildren<MomentumWheel>())
        {
            actualMass += mw.mass;
        }
    }

    public float accelerate(float acc)
    {
        spinSpeed -= acc * Time.fixedDeltaTime * 10;
        return actualMass * acc;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
        if (Mathf.Abs(spinSpeed) > 0.001f)
        {
            //transform.localEulerAngles += axis * Mathf.Clamp(spinSpeed, -maxSpinSpeed, maxSpinSpeed) * Time.fixedDeltaTime * 5;
            transform.Rotate(axis * Mathf.Clamp(spinSpeed, -maxSpinSpeed, maxSpinSpeed) * Time.fixedDeltaTime * 5);
        }
    }
}
