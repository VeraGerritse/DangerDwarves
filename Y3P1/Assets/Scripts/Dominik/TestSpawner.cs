using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class TestSpawner : MonoBehaviourPunCallbacks
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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (aliveDummy)
        {
            PhotonNetwork.Destroy(aliveDummy);
        }
    }
}
