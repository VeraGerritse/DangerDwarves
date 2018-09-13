using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : Item 
{

    [SerializeField] protected int baseDamage;

    public float primaryFireRate;
    public float secondaryFireRate;

    public abstract int CalculateDamage();
}
