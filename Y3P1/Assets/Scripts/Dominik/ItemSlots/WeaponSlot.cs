using Photon.Pun;
using System;
using UnityEngine;
using Y3P1;

public class WeaponSlot : EquipmentSlot
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

    public override void Initialise(bool local)
    {
        base.Initialise(local);

        if (local)
        {
            Player.localPlayer.playerController.OnDodge += PlayerController_OnDodge;
        }
    }

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
            else
            {
                Player.localPlayer.dwarfAnimController.SetMeleeStance(false);
                OnStopChargeSecondary();
                isChargingSecondary = false;
            }

            if (isChargingSecondary)
            {
                secondaryChargeCounter += Time.deltaTime;
            }
        }
        else
        {
            // Just to be safe.
            Player.localPlayer.dwarfAnimController.SetMeleeStance(false);
            OnStopChargeSecondary();
            isChargingSecondary = false;
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
                    Player.localPlayer.dwarfAnimController.SetMeleeStance(canAttack ? true : false);
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (currentWeapon is Weapon_Melee)
            {
                Player.localPlayer.dwarfAnimController.SetMeleeStance(false);
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

    private void PlayerController_OnDodge(bool b)
    {
        canAttack = b ? false : true;
    }

    public void AnimationEventOnUsePrimaryCall()
    {
        OnUsePrimary();
    }

    public void EquipWeapon(Weapon weapon)
    {
        Player.localPlayer.dwarfAnimController.CanEquipRanged();

        int[] ids = Equip(weapon, weapon is Weapon_Ranged ? rangedWeaponSpawn : meleeWeaponSpawn);
        currentWeapon = currentEquipment as Weapon;
        if (currentWeapon != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }

        OnEquipWeapon(weapon);
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        photonView.RPC("ParentWeapon", RpcTarget.AllBuffered, equipmentID, parentID);
    }

    [PunRPC]
    private void ParentWeapon(int equipmentID, int parentID)
    {
        PhotonView pv = PhotonNetwork.GetPhotonView(equipmentID);
        if (pv)
        {
            pv.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
            pv.transform.localPosition = Vector3.zero;
            pv.transform.localRotation = Quaternion.identity;
        }
    }

    public override void OnDisable()
    {
        Player.localPlayer.playerController.OnDodge -= PlayerController_OnDodge;
    }
}
