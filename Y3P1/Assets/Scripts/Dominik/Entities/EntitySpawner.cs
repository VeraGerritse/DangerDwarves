using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class EntitySpawner : MonoBehaviourPunCallbacks, IPunObservable
{

    private bool canSpawn = true;
    private Collider spawnTrigger;

    [SerializeField] private List<GameObject> entityPrefabs = new List<GameObject>();

    [Header("Spawn Settings")]
    [SerializeField] private bool spawnOnAwake;
    [SerializeField] private bool spawnImmortal;
    [SerializeField] private float spawnRange;
    [SerializeField] private float spawnTriggerRange;
    [SerializeField] private float spawnAmount;
    [SerializeField] private GameObject spawnPreview;

    private void Awake()
    {
        if (spawnPreview)
        {
            spawnPreview.SetActive(false);
        }
    }

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

    public void TriggerSpawnManual()
    {
        TriggerSpawn();
    }

    private void TriggerSpawn()
    {
        canSpawn = false;
        if (spawnTrigger)
        {
            spawnTrigger.enabled = false;
        }

        photonView.RPC("SpawnEntities", RpcTarget.All, entityPrefabs[Random.Range(0, entityPrefabs.Count)].name);
    }

    [PunRPC]
    private void SpawnEntities(string entity)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        for (int i = 0; i < spawnAmount; i++)
        {
            Entity newEntity = PhotonNetwork.InstantiateSceneObject(entity, spawnRange == 0 ? transform.position : GetRandomPos(), transform.rotation).GetComponentInChildren<Entity>();
            newEntity.health.isImmortal = spawnImmortal;

            //TODO: Find a way to get rid of this buffered RPC, a cleaner solution is to send the data to whoever connects.
            photonView.RPC("SetEntityInfo", RpcTarget.AllBuffered, newEntity.photonView.ViewID);
        }
    }

    [PunRPC]
    private void SetEntityInfo(int entityID)
    {
        PhotonView pv = PhotonView.Find(entityID);
        if (!pv)
        {
            return;
        }

        Entity entity = pv.GetComponent<Entity>();
        EntityManager.instance.AddToAliveTargets(entity);
    }

    private Vector3 GetRandomPos()
    {
        Vector3 validPos = Vector3.zero;
        bool foundValidPos = false;
        int tries = 0;

        while (!foundValidPos)
        {
            tries++;
            if (tries >= 10)
            {
                Debug.LogWarning("EntitySpawner couldn't find a valid spawn position in " + tries + " tries so it returned Vector.zero");
                return validPos;
            }

            Vector3 randomPos = new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));

            Vector3[] raycastPositions = new Vector3[]
            {
                randomPos + Vector3.up * 0.1f,
                randomPos + Vector3.up * 0.1f + Vector3.right * 0.5f,
                randomPos + Vector3.up * 0.1f + -Vector3.right * 0.5f,
                randomPos + Vector3.up * 0.1f + Vector3.forward * 0.5f,
                randomPos + Vector3.up * 0.1f + -Vector3.forward * 0.5f
            };

            for (int i = 0; i < raycastPositions.Length; i++)
            {
                RaycastHit hit;
                if (Physics.Raycast(raycastPositions[i], Vector3.down, out hit))
                {
                    if (hit.transform.tag != "Environment")
                    {
                        break;
                    }
                    else if (hit.transform.tag == "Environment" && i == raycastPositions.Length - 1 && Mathf.Abs(transform.position.y - hit.point.y) < 0.1f)
                    {
                        foundValidPos = true;
                        validPos = randomPos;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        return validPos;
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
            if (spawnTrigger)
            {
                stream.SendNext(spawnTrigger.enabled);
            }
        }
        else
        {
            canSpawn = (bool)stream.ReceiveNext();
            if (spawnTrigger)
            {
                spawnTrigger.enabled = (bool)stream.ReceiveNext();
            }
        }
    }
}
