using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    public Transform projectileSpawn;

    public void UsePrimary()
    {
        photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation);
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation)
    {
        Projectile newProjectile = ObjectPooler.instance.GrabFromPool("TestProjectile", position, rotation).GetComponent<Projectile>();
        newProjectile.Fire((WeaponSlot.currentWeapon as Weapon_Ranged).force, WeaponSlot.currentWeapon.CalculateDamage());
    }

    public void UseSecondary()
    {

    }
}
