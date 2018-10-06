using Photon.Pun;
using UnityEngine;
using Y3P1;

public class DwarfAnimationsScript : MonoBehaviourPunCallbacks
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

            WeaponSlot.OnUsePrimary += () => myAnim.SetTrigger("FireRanged");
            WeaponSlot.OnUseSecondary += (Weapon.SecondaryType secondaryType) => myAnim.SetTrigger("FireRanged");
            WeaponSlot.OnEquipWeapon += WeaponSlot_OnEquipWeapon;

            Player.localPlayer.playerController.OnDodge += PlayerController_OnDodge;
            Player.localPlayer.entity.OnDeath += () => photonView.RPC("SetAnimationBool", RpcTarget.All, "Dead", true);
            Player.localPlayer.entity.OnRevive += () => photonView.RPC("SetAnimationBool", RpcTarget.All, "Dead", false);
            Player.localPlayer.entity.OnHitEvent.AddListener(() => photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Flinch"));
        }
    }

    private void Awake()
    {
        myAnim = GetComponent<Animator>();
        myIKControl = GetComponent<IKControl>();
    }

    [PunRPC]
    private void SetAnimationTrigger(string trigger)
    {
        myAnim.SetTrigger(trigger);
    }

    [PunRPC]
    private void SetAnimationBool(string name, bool state)
    {
        myAnim.SetBool(name, state);
    }

    private void PlayerController_OnDodge(bool dodgeStart)
    {
        if (dodgeStart)
        {
            myAnim.SetTrigger("Dodge");
        }
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

    public override void OnDisable()
    {
        if (initialised)
        {
            WeaponSlot.OnUsePrimary -= () => myAnim.SetTrigger("FireRanged");
            WeaponSlot.OnUseSecondary -= (Weapon.SecondaryType secondaryType) => myAnim.SetTrigger("FireRanged");
            WeaponSlot.OnEquipWeapon -= WeaponSlot_OnEquipWeapon;

            Player.localPlayer.playerController.OnDodge -= PlayerController_OnDodge;
            Player.localPlayer.entity.OnDeath -= () => myAnim.SetBool("Dead", true);
            Player.localPlayer.entity.OnRevive -= () => myAnim.SetBool("Dead", false);
            Player.localPlayer.entity.OnHitEvent.RemoveAllListeners();
        }
    }

    private void Update()
    {
        if (!initialised || Player.localPlayer.entity.health.isDead)
        {
            return;
        }

        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 combinedAxis = new Vector3(x, 0, y);
        combinedAxis = transform.parent.InverseTransformDirection(combinedAxis);

        myAnim.SetFloat("HorizontalAxis", combinedAxis.x);
        myAnim.SetFloat("VerticalAxis", combinedAxis.z);
        myAnim.SetFloat("Mood", moodSpectrum);
        if(Input.GetKeyDown("z"))
        {
            //myAnim.SetTrigger("Flinch");
            //myAnim.SetBool("Dead", !myAnim.GetBool("Dead"));
        }
    }
}