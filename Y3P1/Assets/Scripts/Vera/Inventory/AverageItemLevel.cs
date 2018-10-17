using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Y3P1;

public class AverageItemLevel : MonoBehaviourPunCallbacks {
    public int averageILevel = 2;
    private float AllLvl;
    private float allPlayers;
    private bool done;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CalculateLevel();

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            NotificationManager.instance.NewNotification(averageILevel.ToString() + "    " + allPlayers.ToString());
        }
    }

    public void CalculateLevel()
    {

            photonView.RPC("GetLevel", RpcTarget.All);
    }


    [PunRPC]
    private void GetLevel()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            AllLvl = 0;
            allPlayers = 0;
        }
            photonView.RPC("Level", RpcTarget.All, Player.localPlayer.myInventory.averageILevel);
    }

    [PunRPC]
    private void Level(int avr)
    {
        AllLvl += avr;
        allPlayers += 1;

        averageILevel = Mathf.RoundToInt(AllLvl / allPlayers);
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification(averageILevel.ToString() + "    " + allPlayers.ToString());
            Pauze();
        }
    }

    private void Pauze()
    {
            photonView.RPC("SetSame", RpcTarget.AllBuffered, averageILevel);
    }

    [PunRPC]
    private void SetSame(int average)
    {
        averageILevel = average;
    }
}
