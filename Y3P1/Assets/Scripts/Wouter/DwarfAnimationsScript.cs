using UnityEngine;

public class DwarfAnimationsScript : MonoBehaviour
{

    public Animator myanim;


    private void Awake()
    {
        WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
    }

    private void WeaponSlot_OnUsePrimary()
    {
        myanim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        myanim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        myanim.SetBool("AimRanged", false);
        myanim.SetBool("bMeleeStance", false);

        if (weapon is Weapon_Ranged)
        {
            myanim.SetBool("AimRanged", true);
        }
        else if(weapon is Weapon_Melee)
        {
            myanim.SetBool("bMeleeStance", true);
        }
    }

    private void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            //myanim.SetTrigger("Melee");
        }
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 combinedAxis = new Vector3(x, 0, y);
        combinedAxis = transform.parent.InverseTransformDirection(combinedAxis);

        myanim.SetFloat("HorizontalAxis", combinedAxis.x);
        myanim.SetFloat("VerticalAxis", combinedAxis.z);

      
    }

}
