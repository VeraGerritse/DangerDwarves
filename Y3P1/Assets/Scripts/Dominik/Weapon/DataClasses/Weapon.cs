using UnityEngine;

[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage = 10;
    public float primaryFireRate = 0.3f;

    [Header("Secondary Attack")]
    public string secondaryProjectile = "Arrow_Homing";
    [Space(10)]
    public float secondaryFireRate = 5;
    public float secondaryForce = 15;
    public int secondaryAmountOfProjectiles = 5;
    public int secondaryConeOfFireInDegrees = 45;

    public abstract int CalculatePrimaryDamage();

    public int CalculateSecondaryDamage()
    {
        return baseDamage + myStats.willpower;
    }
}
