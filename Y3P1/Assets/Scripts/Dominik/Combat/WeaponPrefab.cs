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

    public void UseSecondary()
    {
        StartCoroutine(TestSecondaryBurstFire());
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation)
    {
        Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;

        Projectile newProjectile = ObjectPooler.instance.GrabFromPool(weapon.projectilePoolName, position, rotation).GetComponent<Projectile>();
        newProjectile.Fire(weapon.force, weapon.CalculateDamage());
    }

    private IEnumerator TestSecondaryBurstFire()
    {
        for (int i = 0; i < 3; i++)
        {
            photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation);
            yield return new WaitForSeconds(0.025f);
        }
    }
}
