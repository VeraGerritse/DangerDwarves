using Photon.Pun;
using UnityEngine;

public class TrinketSlot : EquipmentSlot
{

    public static Trinket currentTrinket;

    [SerializeField] private Transform trinketSpawn;

    public void EquipTrinket(Trinket trinket)
    {
        int[] ids = Equip(trinket, trinketSpawn);
        currentTrinket = currentEquipment as Trinket;
        if (currentTrinket != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        photonView.RPC("ParentTrinket", RpcTarget.All, equipmentID, parentID, boc.ObjectToByteArray(currentEquipment));
    }

    [PunRPC]
    private void ParentTrinket(int equipmentID, int parentID, byte[] itemData)
    {
        PhotonView pv = PhotonNetwork.GetPhotonView(equipmentID);
        if (pv)
        {
            pv.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
            pv.transform.localPosition = Vector3.zero;
            pv.transform.localRotation = Quaternion.identity;

            ByteObjectConverter boc = new ByteObjectConverter();
            pv.transform.GetComponent<ItemPrefab>().myItem = (Item)boc.ByteArrayToObject(itemData);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        int[] ids = GetEquipedItemIDs(trinketSpawn);
        ParentEquipment(ids[0], ids[1]);
    }
}
