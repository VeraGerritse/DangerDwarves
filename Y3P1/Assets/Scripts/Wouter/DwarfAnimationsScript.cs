using UnityEngine;

public class DwarfAnimationsScript : MonoBehaviour
{

    private Animator myAnim;
    public IKControl myIKControl;

    private void Awake()
    {
        myAnim = GetComponent<Animator>();
        //myIKControl = GetComponent<IKControl>();

        WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
        WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
        WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;
    }

    private void WeaponSlot_OnUsePrimary()
    {
        myAnim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        myAnim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        myAnim.SetBool("AimRanged", false);
        myAnim.SetBool("bMeleeStance", false);
        if (myIKControl)
        {
            myIKControl.enabled = false;
        }

        if (weapon is Weapon_Ranged)
        {
            myAnim.SetBool("AimRanged", true);
            if (myIKControl)
            {
                myIKControl.enabled = true;
            }
        }
        else if(weapon is Weapon_Melee)
        {
            myAnim.SetBool("bMeleeStance", true);
        }
    }

    public void SetMeleeStance(bool b)
    {
        myAnim.SetBool("bMelee", b);
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

        myAnim.SetFloat("HorizontalAxis", combinedAxis.x);
        myAnim.SetFloat("VerticalAxis", combinedAxis.z);

      
    }

}
