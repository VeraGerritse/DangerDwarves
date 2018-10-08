using UnityEngine;
using Y3P1;

public class AnimationEventsDwarf : MonoBehaviour
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

    public void EndPrimary()
    {
        if (initialised)
        {
            weaponSlot.EndMeleeAnim();
        }
    }

    public void EndDodge()
    {
        if (initialised)
        {
            Player.localPlayer.playerController.EndDodge();
        }
    }
}
