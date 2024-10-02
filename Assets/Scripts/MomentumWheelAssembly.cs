using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomentumWheelAssembly : ShipObject
{
    public GameObject center;
    public MomentumWheel[] wheels;
    public Vector3 wDir;
    public Vector3 aDir;
    public Vector3 sDir;
    public Vector3 dDir;
    public Vector3 qDir;
    public Vector3 eDir;

    public Thruster fakeW;
    public Thruster fakeS;
    public Thruster fakeA;
    public Thruster fakeD;
    public Thruster fakeE;
    public Thruster fakeQ;

    public float[] thrustAmounts = new float[6];

    //[System.NonSerialized]
    public Vector3 targetTorque = Vector3.zero;

    [System.NonSerialized]
    public Vector3 actualTorque = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    int c = 0;
    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();

        if (c % 5 == 0)
        {
            targetTorque = thrustAmounts[0] * wDir + thrustAmounts[1] * sDir + thrustAmounts[2] * aDir + thrustAmounts[3] * dDir + thrustAmounts[4] * eDir + thrustAmounts[5] * qDir;
            //targetTorque = fakeW.thrustAmount * wDir + fakeS.thrustAmount * sDir + fakeA.thrustAmount * aDir + fakeD.thrustAmount * dDir + fakeE.thrustAmount * eDir + fakeQ.thrustAmount * qDir;
            //targetTorque = fakeW.thrustAmount * wDir;
            //Debug.Log("FakeW Thrust = " + fakeW.thrustAmount);

            foreach (MomentumWheel wheel in wheels)
            {
                float amount = Vector3.Dot(ship.transform.InverseTransformDirection(wheel.transform.TransformDirection(wheel.axis)), targetTorque);
                Debug.Log(wheel.gameObject.name + " dot product = " + amount);
                actualTorque += wheel.accelerate(amount) * wheel.axis;
            }

            actualTorque = targetTorque.normalized * Mathf.Clamp(Vector3.Dot(targetTorque, actualTorque) * 5f, 100, 1000);

            for (int i = 0; i < thrustAmounts.Length; i++)
            {
                thrustAmounts[i] = 0;
            }
        }

        if (actualTorque.magnitude > 0.001f)
        {
            ship.rb.AddRelativeTorque(actualTorque);
        }

        c++;
        c %= 10000;
    }
}
