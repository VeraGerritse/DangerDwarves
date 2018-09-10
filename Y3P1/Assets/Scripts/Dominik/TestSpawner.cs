using Photon.Pun;
using UnityEngine;

public class TestSpawner : MonoBehaviourPunCallbacks
{

    public static GameObject aliveDummy;
    private bool canSpawn = true;

    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private float spawnRange;

    //private void Awake()
    //{
    //    TestSpawner[] aliveSpawners = FindObjectsOfType<TestSpawner>();
    //    if (aliveSpawners.Length > 1)
    //    {
    //        Destroy(gameObject);
    //    }
    //}

    private void Update()
    {
        //if (photonView.IsMine)
        //{
            if (!aliveDummy)
            {
                photonView.RPC("SpawnDummy", RpcTarget.All);
            }
        //}
    }

    [PunRPC]
    private void SpawnDummy()
    {
        Health newDummy = PhotonNetwork.Instantiate(dummyPrefab.name, GetRandomPos(), Quaternion.identity).GetComponent<Health>();
        newDummy.OnDeath += () =>
        {
            FindObjectOfType<TestSpawner>().canSpawn = true;
            PhotonNetwork.Destroy(newDummy.gameObject);
        };

        aliveDummy = newDummy.gameObject;
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
    }
}
