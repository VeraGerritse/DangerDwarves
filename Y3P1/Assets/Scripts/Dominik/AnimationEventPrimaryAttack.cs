using UnityEngine;

public class AnimationEventPrimaryAttack : MonoBehaviour
{

    private bool initialised;
    private WeaponSlot weaponSlot;

    private void Awake()
    {
        weaponSlot = GetComponentInParent<WeaponSlot>();
    }

    public void Initialise()
    {
        initialised = true;
    }

    public void UsePrimary()
    {
        if (initialised)
        {
            weaponSlot.AnimationEventOnUsePrimaryCall();
        }
    }
}
