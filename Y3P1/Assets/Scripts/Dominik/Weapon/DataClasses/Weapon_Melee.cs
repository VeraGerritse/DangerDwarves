using UnityEngine;

public class Weapon_Melee : Weapon
{

    [Header("Primary Attack")]
    public float attackRange = 2;

    public override int CalculatePrimaryDamage()
    {
        return baseDamage + myStats.strength;
    }
}
