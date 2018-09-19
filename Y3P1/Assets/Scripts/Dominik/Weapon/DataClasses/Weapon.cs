using UnityEngine;

[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage = 10;
    public float primaryFireRate = 1;

    [Header("Secondary Attack")]
    public string secondaryProjectile = "Arrow_Homing";
    [Space(10)]
    public float secondaryFireRate = 5;
    public float secondaryForce = 1;
    public int secondaryAmountOfProjectiles = 1;
    public int secondaryConeOfFireInDegrees = 0;

    public abstract int CalculatePrimaryDamage();

    public int CalculateSecondaryDamage()
    {
        return baseDamage + myStats.willpower;
    }
}
