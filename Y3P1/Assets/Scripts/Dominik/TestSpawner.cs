using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    public static GameObject aliveDummy;
    private bool canSpawn;

    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private float spawnRange;

    private void Update()
    {
        canSpawn = PhotonNetwork.IsMasterClient ? true : false;

        if (PhotonNetwork.IsMasterClient)
        {
            if (canSpawn && !aliveDummy)
            {
                SpawnDummy();
            }
        }
    }

    private void SpawnDummy()
    {
        Health newDummy = PhotonNetwork.InstantiateSceneObject(dummyPrefab.name, GetRandomPos(), Quaternion.identity).GetComponent<Health>();
        newDummy.OnDeath += () =>
        {
            PhotonNetwork.Destroy(newDummy.gameObject);
            //SpawnDummy();
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
        }
        else
        {
            aliveDummy = (GameObject)stream.ReceiveNext();
        }
    }
}
