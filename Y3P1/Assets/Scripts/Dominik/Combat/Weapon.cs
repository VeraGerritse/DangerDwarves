using UnityEngine;

public abstract class Weapon : Item 
{

    [SerializeField] protected int baseDamage;

    public float primaryFireRate;
    public float secondaryFireRate;

    public abstract int CalculateDamage();
}
