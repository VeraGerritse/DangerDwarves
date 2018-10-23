using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

// THIS SCRIPT IS NOT BEING USED
public class DroppedItemManager : MonoBehaviourPunCallbacks
{

    public static DroppedItemManager instance;

    [SerializeField] private Dictionary<int, Item> droppedItems = new Dictionary<int, Item>();

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    public void RegisterDroppedItem(int itemPrefabID, Item item)
    {
        if (!droppedItems.ContainsKey(itemPrefabID))
        {
            droppedItems.Add(itemPrefabID, item);
        }
    }

    public void RemoveDroppedItem(int itemPrefabID)
    {
        if (droppedItems.ContainsKey(itemPrefabID))
        {
            droppedItems.Remove(itemPrefabID);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        ByteObjectConverter boc = new ByteObjectConverter();

        foreach (KeyValuePair<int, Item> droppedItem in droppedItems)
        {
            byte[] itemToDrop = boc.ObjectToByteArray(droppedItem.Value);
            photonView.RPC("DropItem", RpcTarget.All, droppedItem.Key, itemToDrop);
        }
    }

    [PunRPC]
    private void DropItem(int itemPrefabID, byte[] item)
    {
        ItemPrefab ip = PhotonView.Find(itemPrefabID).GetComponent<ItemPrefab>();

        if (string.IsNullOrEmpty(ip.myItem.itemName))
        {
            ByteObjectConverter boc = new ByteObjectConverter();
            ip.Drop((Item)boc.ByteArrayToObject(item));
        }

        // I tried registering the dropped item here too but that doesnt work.
    }
}
