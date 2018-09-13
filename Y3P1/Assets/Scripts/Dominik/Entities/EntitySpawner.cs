using UnityEngine;
using Photon.Pun;

public class EntitySpawner : MonoBehaviourPunCallbacks
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

    private void Awake()
    {
        if (spawnOnAwake)
        {
            TriggerSpawn();
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
            if (canSpawn && PhotonNetwork.IsMasterClient)
            {
                TriggerSpawn();
            }
        }
    }

    private void TriggerSpawn()
    {
        SpawnEntities();

        canSpawn = false;
        if (spawnTrigger)
        {
            spawnTrigger.enabled = false;
        }
        photonView.RPC("SetCanSpawnRPC", RpcTarget.AllBuffered, false);
    }

    private void SpawnEntities()
    {
        for (int i = 0; i < spawnAmount; i++)
        {
            Entity newEntity = PhotonNetwork.InstantiateSceneObject(entityPrefab.name, GetRandomPos(), transform.rotation).GetComponentInChildren<Entity>();
            newEntity.health.isImmortal = spawnImmortal;
            newEntity.OnDeath += () =>
            {
                PhotonNetwork.Destroy(newEntity.transform.root.gameObject);
            };
        }
    }

    [PunRPC]
    private void SetCanSpawnRPC(bool canSpawn)
    {
        this.canSpawn = canSpawn;
        if (spawnTrigger)
        {
            spawnTrigger.enabled = canSpawn ? true : false;
        }
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnTriggerRange);
    }
}
