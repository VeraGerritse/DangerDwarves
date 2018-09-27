using Photon.Pun;
using UnityEngine;

public class HelmetSlot : EquipmentSlot
{

    public static Helmet currentHelmet;

    [SerializeField] private Transform helmetSpawn;

    public void EquipHelmet(Helmet helmet)
    {
        int[] ids = Equip(helmet, helmetSpawn);
        currentHelmet = currentEquipment as Helmet;
        if (currentHelmet != null)
        {
            ParentEquipment(ids[0], ids[1]);
        }
    }

    protected override void ParentEquipment(int equipmentID, int parentID)
    {
        photonView.RPC("ParentHelmet", RpcTarget.AllBuffered, equipmentID, parentID);
    }

    [PunRPC]
    private void ParentHelmet(int equipmentID, int parentID)
    {
        GameObject equipment = PhotonNetwork.GetPhotonView(equipmentID).gameObject;
        equipment.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
        equipment.transform.localPosition = Vector3.zero;
        equipment.transform.localRotation = Quaternion.identity;
    }
}
