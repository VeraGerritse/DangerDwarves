using Photon.Pun;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{

    private bool canSpawn = true;

    [SerializeField] private GameObject dummyPrefab;
    [SerializeField] private float spawnRange;

    private void Update()
    {
        if (canSpawn)
        {
            SpawnDummy();
        }
    }

    private void SpawnDummy()
    {
        canSpawn = false;

        Health newDummy = PhotonNetwork.Instantiate(dummyPrefab.name, GetRandomPos(), Quaternion.identity).GetComponent<Health>();
        newDummy.OnDeath += () =>
        {
            FindObjectOfType<TestSpawner>().canSpawn = true;
            Destroy(newDummy.gameObject);
        };
    }

    private Vector3 GetRandomPos()
    {
        return new Vector3(transform.position.x + Random.Range(-spawnRange, spawnRange), transform.position.y, transform.position.z + Random.Range(-spawnRange, spawnRange));
    }
}
