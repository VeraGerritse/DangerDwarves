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
        Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;
        photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation, weapon.projectilePoolName, weapon.force, weapon.CalculateDamage());
    }

    private void WeaponSlot_OnUseSecondary()
    {
        StartCoroutine(TestSecondaryBurstFire());
    }

    private void WeaponSlot_OnEquipWeapon()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation, string projectilePoolName, float force, int damage)
    {
        Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
        newProjectile.Fire(force, damage);
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
