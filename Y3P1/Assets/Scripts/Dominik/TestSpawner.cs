using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestSpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    public GameObject aliveDummy;
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
        //Health newDummy = PhotonNetwork.InstantiateSceneObject(dummyPrefab.name, GetRandomPos(), Quaternion.identity).GetComponentInChildren<Health>();
        GameObject newDummy = PhotonNetwork.InstantiateSceneObject(dummyPrefab.name, GetRandomPos(), Quaternion.identity);
        Health newDummyHealth = newDummy.GetComponentInChildren<Health>();
        newDummyHealth.OnDeath += () =>
        {
            PhotonNetwork.Destroy(newDummy);
            //SpawnDummy();
        };

        aliveDummy = newDummy;

        int newDummyID = newDummy.GetComponent<PhotonView>().ViewID;
        photonView.RPC("SyncAliveDummy", RpcTarget.AllBuffered, newDummyID);
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
    }

    // Send the GameObject ID to sync it will all other clients so that when the master client disconnects all other clients will know exactly what to do with the leftover spawn.
    // NOTE: THIS IS NOT A CLEAN AND A TEMPORARY FIX.
    [PunRPC]
    private void SyncAliveDummy(int aliveDummyID)
    {
        aliveDummy = PhotonView.Find(aliveDummyID).gameObject;
        if (aliveDummy)
        {
            aliveDummy.GetComponentInChildren<Health>().OnDeath += () =>
            {
                PhotonNetwork.Destroy(aliveDummy);
            };
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //if (stream.IsWriting)
        //{
        //    stream.SendNext(aliveDummy);
        //}
        //else
        //{
        //    aliveDummy = (GameObject)stream.ReceiveNext();
        //}
    }
}
