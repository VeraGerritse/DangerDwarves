using Photon.Pun;
using UnityEngine;

public class Brazier : MonoBehaviourPunCallbacks
{

    [SerializeField] private GameObject particle;

    public void Light()
    {
        if (!particle.activeInHierarchy)
        {
            photonView.RPC("LightBrazier", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    private void LightBrazier()
    {
        particle.SetActive(true);
    }
}
