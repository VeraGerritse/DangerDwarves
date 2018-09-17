﻿using Photon.Pun;
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
        // Ranged Attack.
        if (WeaponSlot.currentWeapon is Weapon_Ranged)
        {
            Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;
            photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position,
                projectileSpawn.rotation,
                weapon.primaryProjectile,
                weapon.force,
                weapon.CalculatePrimaryDamage(),
                weapon.amountOfProjectiles,
                weapon.coneOfFireInDegrees);
        }
        // Melee Attack.
        else
        {
            Weapon_Melee weapon = WeaponSlot.currentWeapon as Weapon_Melee;
        }
    }

    private void WeaponSlot_OnUseSecondary()
    {
        Weapon weapon = WeaponSlot.currentWeapon;
        photonView.RPC("FireProjectile", RpcTarget.All, projectileSpawn.position,
            projectileSpawn.rotation,
            weapon.secondaryProjectile,
            weapon.secondaryForce,
            weapon.CalculateSecondaryDamage(),
            weapon.secondaryAmountOfProjectiles,
            weapon.secondaryConeOfFireInDegrees);
    }

    private void WeaponSlot_OnEquipWeapon()
    {
        PhotonNetwork.Destroy(gameObject);
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation, string projectilePoolName, float force, int damage, int amountOfProjectiles, int coneOfFireInDegrees)
    {
        // Firing in a straight line.
        if (coneOfFireInDegrees == 0)
        {
            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
                newProjectile.Fire(force, damage);
            }
        }
        // Evenly divide (multiple) projectiles with the cone of fire.
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