using UnityEngine;

[CreateAssetMenu(menuName = "Custom Objects/Ranged Weapon")]
public class Weapon_Ranged : Weapon
{

    [Header("Primary Attack")]
    public string primaryProjectile;
    [Space(10)]
    public float force;
    public int amountOfProjectiles = 1;
    public int coneOfFireInDegrees = 0;

    public override int CalculatePrimaryDamage()
    {
        return baseDamage + myStats.agility;
    }
}
