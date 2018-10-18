using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager_TEST : MonoBehaviourPunCallbacks
{

    public static ItemManager_TEST instance;

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

    private void AddDroppedItem(int itemPrefabID, Item item)
    {
        if (!droppedItems.ContainsKey(itemPrefabID))
        {
            droppedItems.Add(itemPrefabID, item);
        }
    }

    private void RemoveDroppedItem(int itemPrefabID)
    {
        if (droppedItems.ContainsKey(itemPrefabID))
        {
            droppedItems.Remove(itemPrefabID);
        }
    }

    [PunRPC]
    public void SyncDroppedItems(int itemPrefabID, byte[] item, bool add)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        Item i = (Item)boc.ByteArrayToObject(item);

        if (add)
        {
            AddDroppedItem(itemPrefabID, i);
        }
        else
        {
            RemoveDroppedItem(itemPrefabID);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        DropItems();
    }

    [PunRPC]
    private void DropItems()
    {
        foreach (KeyValuePair<int, Item> droppedItem in droppedItems)
        {
            ItemPrefab ip = PhotonView.Find(droppedItem.Key).GetComponent<ItemPrefab>();
            ip.Drop(droppedItem.Value);
        }
    }
}
