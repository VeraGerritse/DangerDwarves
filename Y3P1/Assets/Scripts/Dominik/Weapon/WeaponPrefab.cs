using Photon.Pun;
using UnityEngine;
using Y3P1;

public class WeaponPrefab : ItemPrefab
{

    private Collider[] meleeHits = new Collider[15];

    public Transform projectileSpawn;

    protected override void Awake()
    {
        base.Awake();

        if (photonView.IsMine)
        {
            WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
        }

        projectileSpawn = transform.GetChild(0).transform;
    }

    private void WeaponSlot_OnUsePrimary()
    {
        if (isDropped)
        {
            return;
        }

        // Ranged Attack.
        if (WeaponSlot.currentWeapon is Weapon_Ranged)
        {
            Weapon_Ranged weapon = WeaponSlot.currentWeapon as Weapon_Ranged;
            photonView.RPC("FireProjectile", RpcTarget.All,
                projectileSpawn.position,
                projectileSpawn.rotation,
                weapon.primaryProjectile,
                weapon.force,
                weapon.CalculatePrimaryDamage(),
                weapon.amountOfProjectiles,
                weapon.coneOfFireInDegrees,
                weapon.primaryProjectile == "Arrow_Homing" ? GetClosestTargetViewID() : 9999);
        }
        // Melee Attack.
        else
        {
            Weapon_Melee weapon = WeaponSlot.currentWeapon as Weapon_Melee;

            int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, weapon.attackRange, meleeHits);

            for (int i = 0; i < collidersFound; i++)
            {
                Entity entity = meleeHits[i].GetComponent<Entity>();
                if (entity)
                {
                    if (meleeHits[i].transform.parent.tag != "Player")
                    {
                        Vector3 toHit = meleeHits[i].transform.position - Player.localPlayer.playerController.body.position;
                        if (Vector3.Dot(Player.localPlayer.playerController.body.forward, toHit) > 0)
                        {
                            entity.Hit(-weapon.CalculatePrimaryDamage());

                            if (weapon.knockBack > 0)
                            {
                                Rigidbody rb = entity.transform.parent.GetComponent<Rigidbody>();
                                if (rb)
                                {
                                    rb.AddForce(toHit * weapon.knockBack, ForceMode.Impulse);
                                }
                            }
                        }
                    }
                }
            }

            //RaycastHit hit;
            //if (Physics.Raycast(transform.position, Player.localPlayer.playerController.gameObject.transform.forward, out hit, weapon.attackRange))
            //{
            //    Entity entity = hit.transform.GetComponent<Entity>();
            //    if (entity)
            //    {
            //        entity.Hit(-weapon.CalculatePrimaryDamage());
            //    }
            //}
        }
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        if (isDropped)
        {
            return;
        }

        Weapon weapon = WeaponSlot.currentWeapon;

        int targetID = 9999;
        switch (weapon.secondaryProjectile)
        {
            case "Arrow_Homing":

                targetID = GetClosestTargetViewID();
                break;
            case "Buff_Thorns":

                targetID = Player.localPlayer.photonView.ViewID;
                break;
        }

        photonView.RPC("FireProjectile", RpcTarget.All,
            secondaryType == Weapon.SecondaryType.Attack ? projectileSpawn.position : Player.localPlayer.transform.position,
            secondaryType == Weapon.SecondaryType.Attack ? projectileSpawn.rotation : Player.localPlayer.transform.rotation,
            weapon.secondaryProjectile,
            weapon.secondaryForce,
            weapon.CalculateSecondaryDamage(),
            weapon.secondaryAmountOfProjectiles,
            weapon.secondaryConeOfFireInDegrees,
            targetID);
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        if (isDropped)
        {
            return;
        }

        if (weapon != null)
        {
            myItem = WeaponSlot.currentWeapon;
        }
    }

    [PunRPC]
    private void FireProjectile(Vector3 position, Quaternion rotation, string projectilePoolName, float force, int damage, int amountOfProjectiles, int coneOfFireInDegrees, int targetID = 9999)
    {
        // Firing in a straight line.
        if (coneOfFireInDegrees == 0)
        {
            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
                newProjectile.Fire(force, damage, targetID);
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
                newProjectile.Fire(force, damage, targetID);

                rot.y += angleStep;
            }
        }
    }

    private int GetClosestTargetViewID()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        for (int i = 0; i < EntityManager.instance.aliveTargets.Count; i++)
        {
            // TODO: Maybe move PlayerController.mouseInWorldPos to somewhere else. This feels spaghetti-ish.
            Vector3 directionToTarget = EntityManager.instance.aliveTargets[i].transform.position - PlayerController.mouseInWorldPos;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = EntityManager.instance.aliveTargets[i].transform;
            }
        }

        return bestTarget.GetComponent<PhotonView>().ViewID;
    }

    [PunRPC]
    private void PickUpDestroy()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(gameObject);
        }

        droppedItemLabel.anim.SetTrigger("Pickup");
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
