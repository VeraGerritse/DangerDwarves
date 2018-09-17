using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Y3P1;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    [SerializeField] private GameObject playerPrefab;

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

    private void Start()
    {
        if (!Y3P1.Player.localPlayerObject && playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0.1f, 0), Quaternion.identity);
        }
    }

    public override void OnConnected()
    {
        NotificationManager.instance.NewNotification(PhotonNetwork.NickName + " has entered the hub.");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        NotificationManager.instance.NewNotification(PhotonNetwork.NickName + " has left the hub.");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        SceneManager.instance.LoadScene(0, false);
    }
}
