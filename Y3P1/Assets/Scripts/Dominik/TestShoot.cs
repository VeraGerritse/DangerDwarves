using UnityEngine;
using Photon.Pun;

public class TestShoot : MonoBehaviour 
{

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private float force = 5f;
    [SerializeField] private int damage = 10;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            GetComponent<PhotonView>().RPC("Fire", RpcTarget.All, projectileSpawn.position, projectileSpawn.rotation);
        }

        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Health>().ModifyHealth(-10);
        }
    }

    [PunRPC]
    private void Fire(Vector3 position, Quaternion rotation)
    {
        Projectile newProjectile = Instantiate(projectile, position, rotation).GetComponent<Projectile>();
        newProjectile.Fire(force, damage);
    }
}
