using Photon.Pun;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using Y3P1;

public class WeaponSlot : EquipmentSlot
{

    public static Weapon currentWeapon;

    public static bool canAttack = true;

    public static event Action OnUsePrimary = delegate { };
    public static event Action<Weapon.SecondaryType> OnUseSecondary = delegate { };
    public static event Action<Weapon> OnEquipWeapon = delegate { };

    private bool attackingMelee;
    public static event Action OnStartMelee = delegate { };
    public static event Action OnEndMelee = delegate { };

    public static event Action<float, Weapon> OnStartChargeSecondary = delegate { };
    public static event Action<Weapon> OnStopChargeSecondary = delegate { };

    private float nextPrimaryTime;
    private float currentHits;
    public static int hitsRequiredToSecondary = 15;

    private bool isChargingSecondary;
    private float secondaryChargeCounter;

    public event Action<StatusEffects.StatusEffectType, float> OnWeaponBuffAdded = delegate { };
    public event Action<StatusEffects.StatusEffectType> OnWeaponBuffRemoved = delegate { };

    public static List<WeaponBuff> weaponBuffs = new List<WeaponBuff>();
    public class WeaponBuff
    {
        public StatusEffects.StatusEffectType type;
        public float statusEffectDuration;
        public float endTime;
    }

    [SerializeField] private Transform rangedWeaponSpawn;
    [SerializeField] private Transform meleeWeaponSpawn;
    [SerializeField] private Transform decoyRangedWeaponSpawn;
    [SerializeField] private Transform decoyMeleeWeaponSpawn;

    public override void Initialise(bool local)
    {
        base.Initialise(local);

        if (local)
        {
            Player.localPlayer.playerController.OnDodge += PlayerController_OnDodge;
            Player.localPlayer.entity.OnDeath.AddListener(Entity_OnDeath);

            OnUsePrimary += () =>
            {
                if (!string.IsNullOrEmpty(currentWeapon.secondaryProjectile))
                {
                    currentHits++;
                }
            };
            OnUseSecondary += (Weapon.SecondaryType t) => currentHits = 0;
        }
    }

    private void Update()
    {
        if (CanAttack())
        {
            if (currentWeapon != null && equipedItem != null)
            {
                HandlePrimaryAttack();
                HandleSecondaryAttack();

                if (isChargingSecondary)
                {
                    secondaryChargeCounter += Time.deltaTime;
                }
            }
        }
        else
        {
            StopAllAttacks();
        }

        HandleWeaponBuffTimers();
    }

    private void HandlePrimaryAttack()
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
                    Player.localPlayer.dwarfAnimController.SetMeleeStance(CanAttack() ? true : false);
                    if (!attackingMelee)
                    {
                        OnStartMelee();
                        attackingMelee = true;
                    }
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
    }

    private void HandleSecondaryAttack()
    {
        if (!string.IsNullOrEmpty(currentWeapon.secondaryProjectile))
        {
            if (Input.GetMouseButtonDown(1))
            {
                if (currentHits >= hitsRequiredToSecondary)
                {
                    if (currentWeapon.secondaryChargeupTime == 0)
                    {
                        OnUseSecondary(currentWeapon.secondaryType);
                    }
                    else
                    {
                        if (!isChargingSecondary)
                        {
                            OnStartChargeSecondary(currentWeapon.secondaryChargeupTime, currentWeapon);
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
                        OnStopChargeSecondary(currentWeapon);
                        isChargingSecondary = false;

                        if (secondaryChargeCounter >= currentWeapon.secondaryChargeupTime)
                        {
                            OnUseSecondary(currentWeapon.secondaryType);
                        }
                    }
                }
            }
        }
    }

    private void HandleWeaponBuffTimers()
    {
        for (int i = weaponBuffs.Count - 1; i >= 0; i--)
        {
            if (Time.time >= weaponBuffs[i].endTime)
            {
                OnWeaponBuffRemoved(weaponBuffs[i].type);
                weaponBuffs.Remove(weaponBuffs[i]);
            }
        }
    }

    public void AddBuff(WeaponBuff buff, float duration)
    {
        for (int i = 0; i < weaponBuffs.Count; i++)
        {
            if (weaponBuffs[i].type == buff.type)
            {
                weaponBuffs[i].endTime += duration;
                OnWeaponBuffAdded(buff.type, weaponBuffs[i].endTime - Time.time);
                return;
            }
        }

        weaponBuffs.Add(buff);
        OnWeaponBuffAdded(buff.type, duration);
    }

    private bool CanAttack()
    {
        if (canAttack && !Player.localPlayer.myInventory.InventoryIsOpen() && !UIManager.HasOpenUI && !Player.localPlayer.entity.health.isDead)
        {
            return true;
        }

        return false;
    }

    private void StopAllAttacks()
    {
        Player.localPlayer.dwarfAnimController.SetMeleeStance(false);
        OnStopChargeSecondary(currentWeapon);
        isChargingSecondary = false;
    }

    public void EndMeleeAnim()
    {
        OnEndMelee();
        attackingMelee = false;
    }

    private void PlayerController_OnDodge(bool b)
    {
        canAttack = b ? false : true;
    }

    private void Entity_OnDeath()
    {
        if (isChargingSecondary)
        {
            OnStopChargeSecondary(currentWeapon);
            isChargingSecondary = false;
        }
    }

    public void AnimationEventOnUsePrimaryCall()
    {
        OnUsePrimary();
    }

    public void EquipWeapon(Weapon weapon)
    {
        int[] ids = Equip(weapon, weapon is Weapon_Ranged ? rangedWeaponSpawn : meleeWeaponSpawn);
        DecoyEquip(weapon, weapon is Weapon_Ranged ? decoyRangedWeaponSpawn : decoyMeleeWeaponSpawn);
        currentWeapon = currentEquipment as Weapon;
        if (currentWeapon != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }

        OnEquipWeapon(weapon);
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        photonView.RPC("ParentWeapon", RpcTarget.All, equipmentID, parentID, boc.ObjectToByteArray(currentEquipment));
    }

    [PunRPC]
    private void ParentWeapon(int equipmentID, int parentID, byte[] itemData)
    {
        PhotonView pv = PhotonNetwork.GetPhotonView(equipmentID);
        if (pv)
        {
            pv.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
            pv.transform.localPosition = Vector3.zero;
            pv.transform.localRotation = Quaternion.identity;

            ByteObjectConverter boc = new ByteObjectConverter();
            pv.transform.GetComponent<ItemPrefab>().myItem = (Item)boc.ByteArrayToObject(itemData);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        int[] ids = GetEquipedItemIDs(currentEquipment is Weapon_Ranged ? rangedWeaponSpawn : meleeWeaponSpawn);
        ParentEquipment(ids[0], ids[1]);
    }

    public override void OnDisable()
    {
        Player.localPlayer.playerController.OnDodge -= PlayerController_OnDodge;
        Player.localPlayer.entity.OnDeath.RemoveListener(Entity_OnDeath);
    }
}
