using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Objects/Ranged Weapon")]
public class Weapon_Ranged : Weapon
{

    public string projectilePoolName;
    public float force;

    public override int CalculateDamage()
    {
        return baseDamage;
    }
}
