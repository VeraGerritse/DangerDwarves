using UnityEngine;

public class PickUpTrigger : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                GetComponentInParent<WeaponPrefab>().PickUp();
            }
        }
    }
}
