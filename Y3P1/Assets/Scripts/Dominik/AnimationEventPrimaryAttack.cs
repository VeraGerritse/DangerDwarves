using UnityEngine;

public class AnimationEventPrimaryAttack : MonoBehaviour
{

    private WeaponSlot weaponSlot;

    private void Awake()
    {
        weaponSlot = GetComponentInParent<WeaponSlot>();
    }

    public void UsePrimary()
    {
        weaponSlot.AnimationEventOnUsePrimaryCall();
    }
}
