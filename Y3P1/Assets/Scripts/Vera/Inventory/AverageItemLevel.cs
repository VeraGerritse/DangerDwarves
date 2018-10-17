using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Y3P1;

public class AverageItemLevel : MonoBehaviourPunCallbacks
{

    public int averageILevel;
    private float AllLvl;
    private float allPlayers;
    private bool done;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G) && PhotonNetwork.IsMasterClient)
        {
            CalculateLevel();
        }

        if (Input.GetKeyDown(KeyCode.B) && photonView.IsMine)
        {
            NotificationManager.instance.NewNotification(averageILevel.ToString() + "    " + allPlayers.ToString());
        }
    }

    public void CalculateLevel()
    {
        AllLvl = 0;
        allPlayers = 0;
        photonView.RPC("GetLevel", RpcTarget.All);
    }

    [PunRPC]
    private void GetLevel()
    {
        photonView.RPC("Level", RpcTarget.MasterClient, Player.localPlayer.myInventory.averageILevel);
    }

    [PunRPC]
    private void Level(int avr)
    {
        AllLvl += avr;
        allPlayers = PhotonNetwork.PlayerList.Length;

        averageILevel = Mathf.RoundToInt(AllLvl / allPlayers);

        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification(averageILevel.ToString() + "    " + allPlayers.ToString());

        }
    }
}
