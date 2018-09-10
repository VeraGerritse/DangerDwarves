using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Y3P1;

public class GameManager : MonoBehaviourPunCallbacks
{

    public static GameManager instance;

    [SerializeField] private GameObject playerPrefab;

    [SerializeField] private Vector3 testSpawnerSpawn;
    [SerializeField] private GameObject testSpawner;

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
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0, 2, 0), Quaternion.identity);
        }

        //if (PhotonNetwork.IsMasterClient)
        //{
        //    print("I AM THE MASTER CLIENT");
        //    PhotonNetwork.InstantiateSceneObject(testSpawner.name, testSpawnerSpawn, Quaternion.identity);
        //}

        //TestSpawner newTestSpawner = PhotonNetwork.Instantiate(testSpawner.name, testSpawnerSpawn, Quaternion.identity).GetComponent<TestSpawner>();
        //foreach (TestSpawner testSpawner in FindObjectsOfType<TestSpawner>())
        //{
        //    if (testSpawner != newTestSpawner)
        //    {
        //        print("THIS SHOULD FUCKING DESTROY A SPAWNER");
        //        PhotonNetwork.Destroy(testSpawner.gameObject);
        //    }
        //}
    }

    public void Test()
    {
        PhotonNetwork.InstantiateSceneObject(testSpawner.name, testSpawnerSpawn, Quaternion.identity);
    }

    public override void OnLeftRoom()
    {
        SceneManager.instance.LoadScene(0, false);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void LoadHub()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.LoadLevel(1);
        }
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadHub();
        }
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            LoadHub();
        }
    }
}
