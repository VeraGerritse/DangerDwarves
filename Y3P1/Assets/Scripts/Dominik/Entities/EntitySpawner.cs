using UnityEngine;
using Photon.Pun;

public class EntitySpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    private bool canSpawn = true;
    private Collider spawnTrigger;

    //[SerializeField] private string entityObjectPoolName;
    [SerializeField] private GameObject entityPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnAwake;
    [SerializeField] private bool spawnImmortal;
    [SerializeField] private float spawnRange;
    [SerializeField] private float spawnTriggerRange;
    [SerializeField] private float spawnAmount;

    // Using start to give buffered rpc's a chance to get here before this gets executed.
    private void Start()
    {
        if (spawnOnAwake)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                if (canSpawn)
                {
                    TriggerSpawn();
                }
            }
        }
        else
        {
            SetupSpawnTrigger();
        }
    }

    private void SetupSpawnTrigger()
    {
        if (!GetComponent<Collider>())
        {
            spawnTrigger = gameObject.AddComponent<SphereCollider>();
            spawnTrigger.isTrigger = true;
            (spawnTrigger as SphereCollider).radius = spawnTriggerRange;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (canSpawn)
            {
                TriggerSpawn();
            }
        }
    }

    private void TriggerSpawn()
    {
        canSpawn = false;
        if (spawnTrigger)
        {
            spawnTrigger.enabled = false;
        }

        photonView.RPC("SpawnEntities", RpcTarget.All);
        photonView.RPC("SetCanSpawn", RpcTarget.AllBuffered, false);
    }

    [PunRPC]
    private void SpawnEntities()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            Entity newEntity = PhotonNetwork.InstantiateSceneObject(entityPrefab.name, GetRandomPos(), transform.rotation).GetComponentInChildren<Entity>();
            newEntity.health.isImmortal = spawnImmortal;
            newEntity.OnDeath += () =>
            {
                PhotonNetwork.Destroy(newEntity.transform.root.gameObject);
                //EntityManager.instance.aliveTargets.Remove(newEntity.transform.root.gameObject);
            };

            EntityManager.instance.AddToAliveTargets(newEntity);
        }
    }

    [PunRPC]
    private void SetCanSpawn(bool canSpawn)
    {
        this.canSpawn = canSpawn;
        if (spawnTrigger)
        {
            spawnTrigger.enabled = canSpawn ? true : false;
        }
    }

    private Vector3 GetRandomPos()
    {
        Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));

        Vector3[] raycastPositions = new Vector3[]
        {
            randomPos + Vector3.up * 1,
            randomPos + Vector3.up * 1 + Vector3.right * 0.3f,
            randomPos + Vector3.up * 1 + -Vector3.right * 0.3f,
            randomPos + Vector3.up * 1 + Vector3.forward * 0.3f,
            randomPos + Vector3.up * 1 + -Vector3.forward * 0.3f
        };

        return Vector3.zero;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnTriggerRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRange);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(canSpawn);
        }
        else
        {
            canSpawn = (bool)stream.ReceiveNext();
        }
    }
}
