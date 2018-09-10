using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    private int damage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    public void Fire(float force, int damage)
    {
        this.damage = damage;
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponent<Health>();
        if (health)
        {
            //if (photonView.IsMine)
            //{
                health.ModifyHealth(-damage);
                Destroy(gameObject);
                //photonView.RPC("DestroyProjectile", RpcTarget.All);
            //}
        }
    }

    [PunRPC]
    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}
