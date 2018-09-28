using UnityEngine;
using Y3P1;

public class AnimationEventPrimaryAttack : MonoBehaviour
{

    private bool initialised;
    private WeaponSlot weaponSlot;

    private void Awake()
    {
        //Player.OnLocalPlayerInitialise += Initialise;
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

    private void OnDisable()
    {
        //Player.OnLocalPlayerInitialise -= Initialise;
    }
}
