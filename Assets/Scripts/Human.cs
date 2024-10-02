using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : ShipObject
{
    // Start is called before the first frame update
    new void Start()
    {
        HP = 5;
        maxHP = 5;
        dmgAbsorb = new Yeet.Dmg(0.8f, 0.8f, 0.9f, 0.9f, 0.5f, 0.8f, 0.9f);
    }
    
}
