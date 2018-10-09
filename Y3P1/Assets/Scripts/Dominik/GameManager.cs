using Photon.Pun;
using UnityEngine;
using Y3P1;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject bountyManagerPrefab;

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
        if (!Player.localPlayerObject && playerPrefab)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 0.1f, 0), Quaternion.identity);
        }

        if (PhotonNetwork.IsMasterClient)
        {
            if (!FindObjectOfType<BountyManager>())
            {
                PhotonNetwork.InstantiateSceneObject(bountyManagerPrefab.name, Vector3.zero, Quaternion.identity);
            }
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification(newPlayer.NickName + " has entered the hub.");
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            NotificationManager.instance.NewNotification(otherPlayer.NickName + " has left the hub.");
        }
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
