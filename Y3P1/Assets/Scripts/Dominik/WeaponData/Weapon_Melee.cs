using UnityEngine;

[System.Serializable]
public class Weapon_Melee : Weapon
{

    [Header("Primary Attack")]
    public float attackRange = 2;
    public float knockBack;

    public override int CalculatePrimaryDamage()
    {
        return baseDamage + myStats.strength;
    }

    public override void StartMelee(float range, float knockback)
    {
        attackRange = range;
        knockBack = knockback;
    }
}
