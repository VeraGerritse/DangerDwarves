using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class PlayerStatusCanvas : MonoBehaviour
{

    private HealthBar playerHealthBar;
    private float weaponSecondaryTimer;

    [SerializeField] private Image weaponSecondaryBar;

    private void Start()
    {
        playerHealthBar = GetComponentInChildren<HealthBar>();
        playerHealthBar.Initialise(Player.localPlayer.entity);

        WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
    }

    private void WeaponSlot_OnUseSecondary()
    {
        weaponSecondaryTimer = 0;
    }

    private void Update()
    {
        if (WeaponSlot.currentWeapon)
        {
            weaponSecondaryTimer = (weaponSecondaryTimer < 1) ? weaponSecondaryTimer += 1 / WeaponSlot.currentWeapon.secondaryFireRate * Time.deltaTime : 1;
            weaponSecondaryBar.fillAmount = weaponSecondaryTimer;
        }
        else
        {
            weaponSecondaryBar.fillAmount = 0;
        }
    }

    private void OnDisable()
    {
        WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
    }
}
