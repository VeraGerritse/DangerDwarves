using UnityEngine;

[System.Serializable]
public abstract class Weapon : Item 
{

    [Header("Base Weapon Stats")]
    public int baseDamage = 10;
    public float primaryFireRate = 0.3f;
    public int? materialIndex;

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

    public enum DamageType { Melee, Ranged, Secondary };

    public override string[] WeaponInfo()
    {
        if(secondaryProjectile == "")
        {
            return null;
        }
        string[] wI = new string[] {"Secondary: ", "Attack: <color=#00A8FF>" + ProjectalName(secondaryProjectile), "Arrows: <color=#00A8FF>" + secondaryAmountOfProjectiles.ToString(), "Casting time: <color=#00A8FF>" + secondaryChargeupTime.ToString("F1") };
        return wI;
    }

    string ProjectalName(string name)
    {
        string[] x = name.Split('_');
        return x[1];
    }

    public override string[] DamageInfo()
    {
        return new string[] { "Damage: <color=#00A8FF>" + baseDamage.ToString() };
    }
    public override void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS,bool buff, bool single)
    {
        baseDamage = baseDamage_;
        primaryFireRate = fireRate;
        secondaryProjectile = sS;
        secondaryFireRate = sFR;
        secondaryChargeupTime = charge;
        secondaryForce = fS;
        if (!single)
        {
            secondaryAmountOfProjectiles = aS;
        }
        secondaryConeOfFireInDegrees = dS;
        if (buff)
        {
            secondaryType = SecondaryType.Buff;
        }
    }
}
