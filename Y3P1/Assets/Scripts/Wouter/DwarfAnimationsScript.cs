using UnityEngine;
using Y3P1;

public class DwarfAnimationsScript : MonoBehaviour
{

    private bool initialised;

    private Animator myAnim;
    private IKControl myIKControl;

    public float moodSpectrum;

    public void Initialise(bool local)
    {
        if (local)
        {
            initialised = true;

            WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary += WeaponSlot_OnUseSecondary;
            WeaponSlot.OnStartChargeSecondary += WeaponSlot_OnStartChargeSecondary;
            WeaponSlot.OnStopChargeSecondary += WeaponSlot_OnStopChargeSecondary;
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;

            Player.localPlayer.playerController.OnDodge += PlayerController_OnDodge;
            Player.localPlayer.entity.OnDeath += () => myAnim.SetBool("Dead", true);
            Player.localPlayer.entity.OnRevive += () => myAnim.SetBool("Dead", false);
            Player.localPlayer.entity.OnHit.AddListener(() => myAnim.SetTrigger("Flinch"));
        }
    }


    private void Awake()
    {
        myAnim = GetComponent<Animator>();
        myIKControl = GetComponent<IKControl>();
    }

    private void PlayerController_OnDodge(bool dodgeStart)
    {
        if (dodgeStart)
        {
            myAnim.SetTrigger("Dodge");
        }
    }

    private void WeaponSlot_OnUsePrimary()
    {
        myAnim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnUseSecondary(Weapon.SecondaryType secondaryType)
    {
        myAnim.SetTrigger("FireRanged");
    }

    private void WeaponSlot_OnStartChargeSecondary(float chargeTime, Weapon weapon)
    {
        if (weapon is Weapon_Ranged)
        {
            if (chargeTime < 1)
            {
                myAnim.SetFloat("Chargelenght", 1f);
            }
            else
            {
                myAnim.SetFloat("Chargelenght", 0.5f);
            }

            myAnim.SetBool("RangedAbilityCharging", true);
        }
        else if (weapon is Weapon_Melee)
        {
            if (chargeTime < 1)
            {
                myAnim.SetFloat("Chargelenght", 1.2f);
            }
            else
            {
                myAnim.SetFloat("Chargelenght", 0.8f);
            }

            myAnim.SetBool("MeleeAbilityCharging", true);
        }
    }

    private void WeaponSlot_OnStopChargeSecondary(Weapon weapon)
    {
        myAnim.SetBool("RangedAbilityCharging", false);
        myAnim.SetBool("MeleeAbilityCharging", false);
    }

    private void WeaponSlot_OnEquipWeapon(Weapon weapon)
    {
        myAnim.SetBool("AimRanged", false);
        myAnim.SetBool("bMeleeStance", false);
        myIKControl.enabled = false;

        if (weapon is Weapon_Ranged)
        {
            myAnim.SetBool("AimRanged", true);
            myIKControl.enabled = true;
        }
        else if (weapon is Weapon_Melee)
        {
            myAnim.SetBool("bMeleeStance", true);
        }
    }

    public void SetMeleeStance(bool b)
    {
        myAnim.SetBool("bMelee", b);
    }

    public bool CanEquipRanged()
    {
        if (myAnim.GetCurrentAnimatorStateInfo(3).IsTag("MeleeSwing"))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
            WeaponSlot.OnUseSecondary -= WeaponSlot_OnUseSecondary;
            WeaponSlot.OnStartChargeSecondary -= WeaponSlot_OnStartChargeSecondary;
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;

            Player.localPlayer.playerController.OnDodge -= PlayerController_OnDodge;
            Player.localPlayer.entity.OnDeath -= () => myAnim.SetBool("Dead", true);
            Player.localPlayer.entity.OnRevive -= () => myAnim.SetBool("Dead", false);
            Player.localPlayer.entity.OnHit.RemoveAllListeners();
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
        combinedAxis = !Player.localPlayer.entity.health.isDead ? transform.parent.InverseTransformDirection(combinedAxis) : Vector3.zero;

        myAnim.SetFloat("HorizontalAxis", combinedAxis.x);
        myAnim.SetFloat("VerticalAxis", combinedAxis.z);
        myAnim.SetFloat("Mood", moodSpectrum);

        if (Input.GetKeyDown("z"))
        {
            //myAnim.SetTrigger("Flinch");
            //myAnim.SetBool("Dead", !myAnim.GetBool("Dead"));
            //myAnim.SetBool("RangedAbilityCharging", !myAnim.GetBool("RangedAbilityCharging"));
            //myAnim.SetTrigger("RangedAbilityCharging");
        }
    }
}