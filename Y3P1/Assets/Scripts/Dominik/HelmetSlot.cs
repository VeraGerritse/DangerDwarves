using Photon.Pun;
using UnityEngine;

public class HelmetSlot : MonoBehaviourPunCallbacks
{

    private GameObject equipedHelmet;
    public static Helmet currentHelmet;

    [SerializeField] private Transform helmetSpawn;

    public void EquipHelmet(Helmet helmet)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        if (equipedHelmet)
        {
            PhotonNetwork.Destroy(equipedHelmet);
        }

        if (helmet != null)
        {
            equipedHelmet = PhotonNetwork.Instantiate(Database.hostInstance.allGameobjects[helmet.prefabIndex].name, helmetSpawn.position, helmetSpawn.rotation);

            int currentWeaponPrefabID = equipedHelmet.GetComponent<PhotonView>().ViewID;
            int weaponSpawnID = helmetSpawn.GetComponent<PhotonView>().ViewID;

            photonView.RPC("ParentHelmet", RpcTarget.AllBuffered, currentWeaponPrefabID, weaponSpawnID);
        }
    }

    [PunRPC]
    private void ParentHelmet(int helmetID, int parentID)
    {
        GameObject helmet = PhotonNetwork.GetPhotonView(helmetID).gameObject;
        helmet.transform.SetParent(PhotonNetwork.GetPhotonView(parentID).transform);
        helmet.transform.localPosition = Vector3.zero;
        helmet.transform.localRotation = Quaternion.identity;
    }
}
