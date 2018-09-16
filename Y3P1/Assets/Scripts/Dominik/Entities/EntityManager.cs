using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviourPunCallbacks
{

    public static EntityManager instance;

    public List<Entity> aliveTargets = new List<Entity>();

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

    public void AddToAliveTargets(Entity entity)
    {
        aliveTargets.Add(entity);
        photonView.RPC("SyncAliveTargets", RpcTarget.AllBuffered, entity.gameObject.GetPhotonView().ViewID);
    }

    [PunRPC]
    private void SyncAliveTargets(int photonViewID)
    {
        Entity target = PhotonView.Find(photonViewID).gameObject.GetComponent<Entity>();
        if (target)
        {
            if (!aliveTargets.Contains(target))
            {
                aliveTargets.Add(target);
            }
        }
    }
}
