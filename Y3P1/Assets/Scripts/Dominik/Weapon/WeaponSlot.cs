using Photon.Pun;
using UnityEngine;
using System;

public class WeaponSlot : MonoBehaviourPunCallbacks
{

    public static Weapon currentWeapon;

    public static event Action OnUsePrimary = delegate { };
    public static event Action OnUseSecondary = delegate { };
    public static event Action OnEquipWeapon = delegate { };

    private float nextPrimaryTime;
    private float nextSecondaryTime;

    [SerializeField] private Transform weaponSpawn;

    private void Update()
    {
        if (UIManager.hasOpenUI)
        {
            return;
        }

        if (currentWeapon != null)
        {
            HandleWeaponActions();
        }
    }

    private void HandleWeaponActions()
    {
        if (Input.GetMouseButton(0))
        {
            if (Time.time >= nextPrimaryTime)
            {
                nextPrimaryTime = Time.time + currentWeapon.primaryFireRate;
                OnUsePrimary();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (!string.IsNullOrEmpty(currentWeapon.secondaryProjectile))
            {
                if (Time.time >= nextSecondaryTime)
                {
                    nextSecondaryTime = Time.time + currentWeapon.secondaryFireRate;
                    OnUseSecondary();
                }
            }
        }
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        OnEquipWeapon();

        currentWeapon = weapon;
        if (weapon != null)
        {
            GameObject currentWeaponPrefab = PhotonNetwork.Instantiate(Database.hostInstance.allGameobjects[currentWeapon.prefabIndex].name, weaponSpawn.position, weaponSpawn.rotation);
            currentWeaponPrefab.transform.SetParent(weaponSpawn);
        }
    }
}
