using Photon.Pun;
using UnityEngine;

public class HelmetSlot : EquipmentSlot
{

    public static Helmet currentHelmet;

    [SerializeField] private Transform helmetSpawn;
    [SerializeField] private Transform decoyHelmetSpawn;

    public void EquipHelmet(Helmet helmet)
    {
        int[] ids = Equip(helmet, helmetSpawn);
        DecoyEquip(helmet, helmetSpawn);
        currentHelmet = currentEquipment as Helmet;
        if (currentHelmet != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        ByteObjectConverter boc = new ByteObjectConverter();
        photonView.RPC("ParentHelmet", RpcTarget.All, equipmentID, parentID, boc.ObjectToByteArray(currentEquipment));
    }

    [PunRPC]
    private void ParentHelmet(int equipmentID, int parentID, byte[] itemData)
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
        int[] ids = GetEquipedItemIDs(helmetSpawn);
        ParentEquipment(ids[0], ids[1]);
    }
}
