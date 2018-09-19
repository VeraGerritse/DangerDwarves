using UnityEngine;
[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage;
    public float primaryFireRate;

    [Header("Secondary Attack")]
    public string secondaryProjectile;
    [Space(10)]
    public float secondaryFireRate;
    public float secondaryForce;
    public int secondaryAmountOfProjectiles = 1;
    public int secondaryConeOfFireInDegrees = 0;

    public abstract int CalculatePrimaryDamage();

    public int CalculateSecondaryDamage()
    {
        return baseDamage + myStats.willpower;
    }
}
