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

    private void WeaponSlot_OnUseSecondary()
    {
        myanim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        myanim.SetBool("AimRanged", weapon != null ? true : false);
    }

    private void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
    }

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 combinedAxis = new Vector3(x, 0, y);
        combinedAxis = transform.parent.InverseTransformDirection(combinedAxis);

        myanim.SetFloat("HorizontalAxis", combinedAxis.x);
        myanim.SetFloat("VerticalAxis", combinedAxis.z);
    }
}
