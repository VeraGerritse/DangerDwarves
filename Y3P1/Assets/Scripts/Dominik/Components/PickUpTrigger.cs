using UnityEngine;
using Y3P1;

public class PickUpTrigger : MonoBehaviour
{

    private WeaponPrefab myWeaponPrefab;

    private void Awake()
    {
        myWeaponPrefab = GetComponentInParent<WeaponPrefab>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!Player.localPlayer.myInventory.CheckFull())
                {
                    myWeaponPrefab.PickUp();
                }
            }
        }
    }
}
