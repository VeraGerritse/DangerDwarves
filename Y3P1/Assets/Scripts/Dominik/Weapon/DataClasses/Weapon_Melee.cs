using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Melee : Weapon
{
    public override int CalculatePrimaryDamage()
    {
        return baseDamage + myStats.strength;
    }
}
