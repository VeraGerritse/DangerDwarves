using Photon.Pun;
using UnityEngine;

public abstract class EquipmentSlot : MonoBehaviourPunCallbacks
{

    protected Item currentEquipment;
    protected GameObject equipedItem;

    // Equips the item 'toEquip' at 'spawnpoint' and returns two PhotonView ID's to use for parenting the item via an RPC.
    protected int[] Equip(Item toEquip, Transform spawnpoint)
    {
        if (!photonView.IsMine)
        {
            return null;
        }

        if (equipedItem)
        {
            PhotonNetwork.Destroy(equipedItem);
        }

        currentEquipment = toEquip;

        if (toEquip != null)
        {
            if (spawnpoint)
            {
                equipedItem = PhotonNetwork.Instantiate(Database.hostInstance.allGameobjects[toEquip.prefabIndex].name, spawnpoint.position, spawnpoint.rotation);
            }

            int equipmentID = equipedItem.GetComponent<PhotonView>().ViewID;
            int spawnpointID = spawnpoint ? spawnpoint.GetComponent<PhotonView>().ViewID : 0;

            return new int[] { equipmentID, spawnpointID };
        }

        return null;
    }

    protected abstract void ParentEquipment(int equipmentID, int parentID);
}
