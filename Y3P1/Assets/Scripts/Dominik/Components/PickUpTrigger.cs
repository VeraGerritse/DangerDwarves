using UnityEngine;
using Y3P1;

public class PickUpTrigger : MonoBehaviour
{

    private bool checkForInput;
    private ItemPrefab myItemPrefab;

    private void Awake()
    {
        myItemPrefab = GetComponentInParent<ItemPrefab>();
    }

    private void Update()
    {
        if (checkForInput)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!Player.localPlayer.myInventory.CheckFull())
                {
                    myItemPrefab.PickUp();
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                checkForInput = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.gameObject.layer == 9)
            {
                checkForInput = false;
            }
        }
    }
}
