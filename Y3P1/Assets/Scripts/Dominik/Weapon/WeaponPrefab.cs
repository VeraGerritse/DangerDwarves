using Photon.Pun;
using UnityEngine;

public class WeaponPrefab : MonoBehaviourPunCallbacks
{

    private Collider interactCollider;
    private Collider objectCollider;
    private Rigidbody rb;
    public Transform projectileSpawn;
    public Item myItem;

    private void Awake()
    {
        interactCollider = GetComponent<Collider>();
        objectCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();

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
                weapon.coneOfFireInDegrees,
                weapon.primaryProjectile == "Arrow_Homing" ? GetClosestTargetViewID() : 9999);
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
            weapon.secondaryConeOfFireInDegrees,
            weapon.secondaryProjectile == "Arrow_Homing" ? GetClosestTargetViewID() : 9999);
    }

    private void WeaponSlot_OnEquipWeapon()
    {
        PhotonNetwork.Destroy(gameObject);
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

    public void Drop()
    {
        interactCollider.enabled = true;
        objectCollider.enabled = true;
        rb.isKinematic = false;
        rb.AddForce(Vector3.up * Random.Range(1, 3), ForceMode.Impulse);

        DroppedItemLabel newLabel = ObjectPooler.instance.GrabFromPool("DroppedItemLabel", transform.position, Quaternion.identity).GetComponent<DroppedItemLabel>();
        newLabel.SetText(WeaponSlot.currentWeapon.itemName, WeaponSlot.currentWeapon.itemRarity);
    }

    public void PickUp()
    {
        interactCollider.enabled = false;
        objectCollider.enabled = false;
        rb.isKinematic = true;
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
