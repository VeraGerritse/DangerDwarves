using Photon.Pun;
using UnityEngine;

public class TestSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    public static GameObject aliveDummy;
    //private bool initialised;

    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private float spawnRange;

    private void Awake()
    {
        //if (initialised)
        //{
        //    print("test spawner is already initialised");
        //    return;
        //}

        //initialised = true;

        if (!aliveDummy)
        {
            SpawnDummy();
        }
    }

    private void SpawnDummy()
    {
        Health newDummy = PhotonNetwork.Instantiate(dummyPrefab.name, GetRandomPos(), Quaternion.identity).GetComponent<Health>();
        newDummy.OnDeath += () =>
        {
            PhotonNetwork.Destroy(newDummy.gameObject);
            SpawnDummy();
        };

        aliveDummy = newDummy.gameObject;
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(aliveDummy);
            //stream.SendNext(initialised);
        }
        else
        {
            aliveDummy = (GameObject)stream.ReceiveNext();
            //initialised = (GameObject)stream.ReceiveNext();
        }
    }
}
