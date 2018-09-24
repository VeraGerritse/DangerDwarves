using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Custom Objects/Ranged Weapon")]
public class Weapon_Ranged : Weapon
{

    [Header("Primary Attack")]
    public string primaryProjectile = "Arrow";
    [Space(10)]
    public float force = 20;
    public int amountOfProjectiles = 1;
    public int coneOfFireInDegrees = 0;

    public override int CalculatePrimaryDamage()
    {
        return baseDamage + myStats.agility;
    }

    public override void StartRanged(float fP, int aP, int dP)
    {
        force = fP;
        amountOfProjectiles = aP;
        coneOfFireInDegrees = dP;
    }
}
