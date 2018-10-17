using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Y3P1;

public class AverageItemLevel : MonoBehaviourPunCallbacks {
    public int averageILevel;
    public int test;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            CalculateLevel();
            NotificationManager.instance.NewNotification(test.ToString());
        }
    }

    public void CalculateLevel()
    {
        photonView.RPC("GetLevel", RpcTarget.All);
    }

    [PunRPC]
    private void GetLevel()
    {
        test += 1;
    }
}
