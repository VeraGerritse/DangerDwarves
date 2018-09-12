using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    private int damage;

    [SerializeField] private string myPoolName;

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
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            if (photonView.IsMine)
            {
                entity.Hit(-damage);
            }

            ObjectPooler.instance.AddToPool(myPoolName, gameObject);
        }
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
    }
}
