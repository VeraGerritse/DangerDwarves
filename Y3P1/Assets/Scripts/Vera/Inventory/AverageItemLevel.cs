using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Y3P1;

public class AverageItemLevel : MonoBehaviourPunCallbacks {
    public int averageILevel = 2;
    private float AllLvl;
    private float allPlayers;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            NotificationManager.instance.NewNotification(averageILevel.ToString());
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
        photonView.RPC("Level", RpcTarget.All, Player.localPlayer.myInventory.averageILevel);
    }

    [PunRPC]
    private void Level(int avr)
    {
        AllLvl += avr;
        allPlayers += 1;

        averageILevel = Mathf.RoundToInt(AllLvl / allPlayers);
    }
}
