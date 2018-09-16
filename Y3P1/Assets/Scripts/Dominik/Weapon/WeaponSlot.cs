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
    [SerializeField] private Weapon defaultWeapon;

    [SerializeField] private Weapon testWeaponSwitch;

    private void Awake()
    {
        if (defaultWeapon)
        {
            EquipWeapon(defaultWeapon);
        }
    }

    private void Update()
    {
        if (UIManager.hasOpenUI)
        {
            return;
        }

        HandleWeaponActions();

        if (Input.GetButtonDown("Jump"))
        {
            EquipWeapon(currentWeapon == defaultWeapon ? testWeaponSwitch : defaultWeapon);
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
        GameObject currentWeaponPrefab = PhotonNetwork.Instantiate(currentWeapon.itemPrefab.name, weaponSpawn.position, weaponSpawn.rotation);
        currentWeaponPrefab.transform.SetParent(weaponSpawn);
    }
}
