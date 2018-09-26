using UnityEngine;
using Y3P1;

public class PickUpTrigger : MonoBehaviour
{

    private ItemPrefab myItemPrefab;

    private void Awake()
    {
        myItemPrefab = GetComponentInParent<ItemPrefab>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!Player.localPlayer.myInventory.CheckFull())
                {
                    print("test");
                    myItemPrefab.PickUp();
                }
            }
        }
    }
}
