using UnityEngine;

public class PickUpTrigger : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Check if inventory has space.
                GetComponentInParent<WeaponPrefab>().PickUp();
            }
        }
    }
}
