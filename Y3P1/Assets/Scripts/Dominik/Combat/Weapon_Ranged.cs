using UnityEngine;

[CreateAssetMenu(menuName = "Custom Objects/Ranged Weapon")]
public class Weapon_Ranged : Weapon
{

    public string projectilePoolName;
    public float force;
    public int amountOfProjectiles = 1;
    public int coneOfFireInDegrees = 0;

    public override int CalculateDamage()
    {
        return baseDamage;
    }
}
