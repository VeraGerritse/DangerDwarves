using UnityEngine;

public class PvPZone : MonoBehaviour 
{

    private void OnTriggerEnter(Collider other)
    {
        Projectile projectile = other.GetComponent<Projectile>();
        if (projectile)
        {
            projectile.damageTarget = Projectile.Target.Player;
        }
    }
}
