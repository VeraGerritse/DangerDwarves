using UnityEngine;
using Photon.Pun;

public class TestShoot : MonoBehaviour 
{

    [SerializeField] private GameObject projectile;
    [SerializeField] private Transform projectileSpawn;
    [SerializeField] private float force = 5f;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Projectile newProjectile = PhotonNetwork.Instantiate(projectile.name, projectileSpawn.position, projectileSpawn.rotation).GetComponent<Projectile>();
            newProjectile.Fire(force);
        }

        if (Input.GetButtonDown("Jump"))
        {
            GetComponent<Health>().ModifyHealth(-10);
        }
    }
}
