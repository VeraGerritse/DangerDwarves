using UnityEngine;

[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage = 10;
    public float primaryFireRate = 0.3f;

    [Header("Secondary Attack")]
    public string secondaryProjectile = "Arrow_Animated";
    [Space(10)]
    public float secondaryFireRate = 3;
    public float secondaryForce = 15;
    public int secondaryAmountOfProjectiles = 1;
    public int secondaryConeOfFireInDegrees = 0;

    public abstract int CalculatePrimaryDamage();

    public int CalculateSecondaryDamage()
    {
        return baseDamage + myStats.willpower;
    }
}
