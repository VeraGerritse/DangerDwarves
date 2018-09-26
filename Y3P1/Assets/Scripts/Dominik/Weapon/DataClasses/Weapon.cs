using UnityEngine;

[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage = 10;
    public float primaryFireRate = 0.3f;

    [Header("Secondary Attack")]
    public string secondaryProjectile = "Arrow_Animated";
    public enum SecondaryType { Attack, Buff };
    public SecondaryType secondaryType;
    [Space(10)]
    public float secondaryFireRate = 3;
    public float secondaryChargeupTime = 1f;
    public float secondaryForce = 15;
    public int secondaryAmountOfProjectiles = 1;
    public int secondaryConeOfFireInDegrees = 0;

    public abstract int CalculatePrimaryDamage();

    public int CalculateSecondaryDamage()
    {
        return baseDamage + myStats.willpower;
    }

    public override string[] WeaponInfo()
    {
        if(secondaryProjectile == "")
        {
            return new string[] {"damage: <color=red>" + CalculatePrimaryDamage().ToString() };
        }
        string[] wI = new string[] { "damage: <color=red>" + CalculatePrimaryDamage().ToString(), "Secondary: " + secondaryProjectile, "Projectiles in secondary: " + secondaryAmountOfProjectiles.ToString(), "Casting time: " + secondaryChargeupTime.ToString() };
        return wI;
    }
    public override void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS,bool buff)
    {
        baseDamage = baseDamage_;
        primaryFireRate = fireRate;
        secondaryProjectile = sS;
        secondaryFireRate = sFR;
        secondaryChargeupTime = charge;
        secondaryForce = fS;
        secondaryAmountOfProjectiles = aS;
        secondaryConeOfFireInDegrees = dS;
        if (buff)
        {
            secondaryType = SecondaryType.Buff;
        }
    }
}
