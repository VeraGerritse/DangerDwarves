using Photon.Pun;
using System;
using UnityEngine;
using Y3P1;

public class WeaponSlot : MonoBehaviourPunCallbacks
{

    public static Weapon currentWeapon;

    public static bool canAttack;

    public static event Action OnUsePrimary = delegate { };
    public static event Action<Weapon.SecondaryType> OnUseSecondary = delegate { };
    public static event Action<Weapon> OnEquipWeapon = delegate { };

    public static event Action<float> OnStartChargeSecondary = delegate { };
    public static event Action OnStopChargeSecondary = delegate { };

    private float nextPrimaryTime;
    private float nextSecondaryTime;

    private bool isChargingSecondary;
    private float secondaryChargeCounter;

    [SerializeField] private Transform rangedWeaponSpawn;
    [SerializeField] private Transform meleeWeaponSpawn;

    private void Update()
    {
        if (UIManager.hasOpenUI)
        {
            return;
        }

        if (currentWeapon != null)
        {
            if (canAttack)
            {
                HandleWeaponActions();
            }

            if (isChargingSecondary)
            {
                secondaryChargeCounter += Time.deltaTime;
            }
        }
    }

    private void HandleWeaponActions()
    {
        if (Input.GetMouseButton(0))
        {
            if (!isChargingSecondary)
            {
                if (currentWeapon is Weapon_Ranged)
                {
                    if (Time.time >= nextPrimaryTime)
                    {
                        nextPrimaryTime = Time.time + currentWeapon.primaryFireRate;
                        OnUsePrimary();
                    }
                }
                else
                {
                    Player.localPlayer.dwarfAnimController.myanim.SetBool("bMelee", true);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentWeapon is Weapon_Melee)
            {
                Player.localPlayer.dwarfAnimController.myanim.SetBool("bMelee", false);
            }
        }

        if (!string.IsNullOrEmpty(currentWeapon.secondaryProjectile))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (Time.time >= nextSecondaryTime)
                {
                    if (currentWeapon.secondaryChargeupTime == 0)
                    {
                        nextSecondaryTime = Time.time + currentWeapon.secondaryFireRate;
                        OnUseSecondary(currentWeapon.secondaryType);
                    }
                    else
                    {
                        if (!isChargingSecondary)
                        {
                            OnStartChargeSecondary(currentWeapon.secondaryChargeupTime);
                            isChargingSecondary = true;
                            secondaryChargeCounter = 0;
                        }
                    }
                }
            }

            if (Input.GetMouseButtonUp(1))
            {
                if (currentWeapon.secondaryChargeupTime > 0)
                {
                    if (isChargingSecondary)
                    {
                        OnStopChargeSecondary();
                        isChargingSecondary = false;

                        if (secondaryChargeCounter >= currentWeapon.secondaryChargeupTime)
                        {
                            nextSecondaryTime = Time.time + currentWeapon.secondaryFireRate;
                            OnUseSecondary(currentWeapon.secondaryType);
                        }
                    }
                }
            }
        }
    }

    public void AnimationEventOnUsePrimaryCall()
    {
        OnUsePrimary();
    }

    public void EquipWeapon(Weapon weapon)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        OnEquipWeapon(weapon);

        currentWeapon = weapon;
        if (weapon != null)
        {
            Transform weaponSpawn = weapon is Weapon_Ranged ? rangedWeaponSpawn : meleeWeaponSpawn;

            GameObject currentWeaponPrefab = PhotonNetwork.Instantiate(Database.hostInstance.allGameobjects[currentWeapon.prefabIndex].name, weaponSpawn.position, weaponSpawn.rotation);

            int currentWeaponPrefabID = currentWeaponPrefab.GetComponent<PhotonView>().ViewID;
            int weaponSpawnID = weaponSpawn.GetComponent<PhotonView>().ViewID;

            photonView.RPC("ParentWeapon", RpcTarget.AllBuffered, currentWeaponPrefabID, weaponSpawnID);
        }
    }

    [PunRPC]
    public void ParentWeapon(int weaponID, int parentID)
    {
        GameObject weapon = PhotonNetwork.GetPhotonView(weaponID).gameObject;
        weapon.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
    }
}
