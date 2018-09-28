using UnityEngine;
using Y3P1;

public class DwarfAnimationsScript : MonoBehaviour
{

    private bool initialised;

    private Animator myAnim;
    private IKControl myIKControl;

    private void Awake()
    {
        Player.OnLocalPlayerInitialise += Initialise;

        myAnim = GetComponent<Animator>();
        myIKControl = GetComponent<IKControl>();
    }

    private void Initialise()
    {
        initialised = true;

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
        Player.OnLocalPlayerInitialise -= Initialise;

        if (initialised)
        {
            WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;
        }
    }

    private void Update()
    {
        if (!initialised)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 combinedAxis = new Vector3(x, 0, y);
        combinedAxis = transform.parent.InverseTransformDirection(combinedAxis);

        myAnim.SetFloat("HorizontalAxis", combinedAxis.x);
        myAnim.SetFloat("VerticalAxis", combinedAxis.z);

        if(Input.GetButtonDown("Jump"))
        {
            myAnim.SetTrigger("Dodge");
        }
    }
}
