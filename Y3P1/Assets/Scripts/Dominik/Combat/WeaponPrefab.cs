using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    public Transform projectileSpawn;

    private void Awake()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
        }
    }

    private void WeaponSlot_OnUsePrimary()
    {
        // Asumes the equipped weapon is a ranged weapon.
        Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;
        photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation, weapon.projectilePoolName, weapon.force, weapon.CalculateDamage(), weapon.amountOfProjectiles, weapon.coneOfFireInDegrees);
    }

    private void WeaponSlot_OnUseSecondary()
    {
        //StartCoroutine(TestSecondaryBurstFire());
    }

    private void WeaponSlot_OnEquipWeapon()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation, string projectilePoolName, float force, int damage, int amountOfProjectiles, int coneOfFireInDegrees)
    {
        if (coneOfFireInDegrees == 0 || amountOfProjectiles == 1)
        {
            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
                newProjectile.Fire(force, damage);
            }
        }
        else
        {
            float angleStep = coneOfFireInDegrees / amountOfProjectiles;
            float startingAngle = coneOfFireInDegrees / 2 - angleStep / 2;

            Vector3 rot = rotation.eulerAngles;
            rot.y -= startingAngle;

            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, Quaternion.Euler(rot)).GetComponent<Projectile>();
                newProjectile.Fire(force, damage);

                rot.y += angleStep;
            }
        }
    }

    private IEnumerator TestSecondaryBurstFire()
    {
        for (int i = 0; i < 3; i++)
        {
            Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;
            photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation, weapon.projectilePoolName, weapon.force, weapon.CalculateDamage());

            yield return new WaitForSeconds(0.025f);
        }
    }

    public override void OnDisable()
    {
        if (photonView.IsMine)
        {
            WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
        }
    }
}
