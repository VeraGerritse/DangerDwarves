using UnityEngine;

[System.Serializable]
public class Weapon_Melee : Weapon
{

    [Header("Primary Attack")]
    public float attackRange = 2;
    public float knockBack;

    public override void StartMelee(float range, float knockback)
    {
        attackRange = range;
        knockBack = knockback;
    }
}
