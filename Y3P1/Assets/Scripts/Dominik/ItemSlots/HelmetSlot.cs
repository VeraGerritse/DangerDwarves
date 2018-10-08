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
        photonView.RPC("ParentHelmet", RpcTarget.AllBuffered, equipmentID, parentID);
    }

    [PunRPC]
    private void ParentHelmet(int equipmentID, int parentID)
    {
        PhotonView pv = PhotonNetwork.GetPhotonView(equipmentID);
        if (pv)
        {
            pv.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
            pv.transform.localPosition = Vector3.zero;
            pv.transform.localRotation = Quaternion.identity;
        }
    }
}
