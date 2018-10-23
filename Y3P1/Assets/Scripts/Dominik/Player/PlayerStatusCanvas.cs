using UnityEngine;
using UnityEngine.UI;
using Y3P1;
using TMPro;
using System.Collections.Generic;

public class PlayerStatusCanvas : MonoBehaviour
{

    private HealthBar playerHealthBar;
    private float weaponSecondaryTimer;
    private float weaponSecondaryBarFill;

    [SerializeField] private Image weaponSecondaryBar;
    [SerializeField] private GameObject localPlayerInfoPanel;
    [SerializeField] private TextMeshProUGUI localPlayerNameText; 
    [SerializeField] private TextMeshProUGUI localPlayerHealthText;
    [SerializeField] private GameObject noSecondaryText;

    [Space(10)]

    [SerializeField] private List<StatusEffectIcon> weaponBuffIcons = new List<StatusEffectIcon>();

    public void Start()
    {
        playerHealthBar = GetComponentInChildren<HealthBar>();
        playerHealthBar.Initialise(Player.localPlayer.entity);

        InitialiseEvents();

        weaponSecondaryBar.fillAmount = 0;
    }

    private void InitialiseEvents()
    {
        WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;

        Player.localPlayer.weaponSlot.OnWeaponBuffAdded += WeaponSlot_OnWeaponBuffAdded;
        Player.localPlayer.weaponSlot.OnWeaponBuffRemoved += WeaponSlot_OnWeaponBuffRemoved;
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        if (weapon != null && !string.IsNullOrEmpty(weapon.secondaryProjectile))
        {
            noSecondaryText.SetActive(false);
        }
        else
        {
            noSecondaryText.SetActive(true);
        }
    }

    private void WeaponSlot_OnUsePrimary()
    {
        weaponSecondaryBarFill += 1 / (float)WeaponSlot.hitsRequiredToSecondary;
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        weaponSecondaryBarFill = 0;
        weaponSecondaryBar.fillAmount = 0;
    }

    private void Update()
    {
        if (weaponSecondaryBar.fillAmount < 1)
        {
            weaponSecondaryBar.fillAmount = Mathf.Lerp(weaponSecondaryBar.fillAmount, weaponSecondaryBarFill, Time.deltaTime * 10);
        }

        if (localPlayerInfoPanel.activeInHierarchy)
        {
            localPlayerInfoPanel.transform.position = Input.mousePosition;
        }
    }

    public void TogglePlayerInfoPanel(bool toggle)
    {
        if (toggle)
        {
            localPlayerNameText.text = Player.localPlayer.photonView.Owner.NickName;
            localPlayerHealthText.text = "HP: " + Player.localPlayer.entity.health.GetHealthString();
        }

        localPlayerInfoPanel.SetActive(toggle);
    }

    private void WeaponSlot_OnWeaponBuffAdded(StatusEffects.StatusEffectType type, float duration)
    {
        ToggleWeaponBuffIcon(type, true, duration);
    }

    private void WeaponSlot_OnWeaponBuffRemoved(StatusEffects.StatusEffectType type)
    {
        ToggleWeaponBuffIcon(type, false);
    }

    private void ToggleWeaponBuffIcon(StatusEffects.StatusEffectType type, bool toggle, float? duration = null)
    {
        for (int i = 0; i < weaponBuffIcons.Count; i++)
        {
            if (weaponBuffIcons[i].type == type)
            {
                if (toggle)
                {
                    weaponBuffIcons[i].Activate(duration);
                }
                else
                {
                    weaponBuffIcons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    private void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;

        Player.localPlayer.weaponSlot.OnWeaponBuffAdded -= WeaponSlot_OnWeaponBuffAdded;
        Player.localPlayer.weaponSlot.OnWeaponBuffRemoved -= WeaponSlot_OnWeaponBuffRemoved;
    }
}
