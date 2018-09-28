using UnityEngine;
using Y3P1;

public class AnimationEventPrimaryAttack : MonoBehaviour
{

    private bool initialised;
    private WeaponSlot weaponSlot;

    public void Initialise(bool local)
    {
        if (local)
        {
            weaponSlot = GetComponentInParent<WeaponSlot>();
            initialised = true;
        }
    }

    public void UsePrimary()
    {
        if (initialised)
        {
            weaponSlot.AnimationEventOnUsePrimaryCall();
        }
    }
}
