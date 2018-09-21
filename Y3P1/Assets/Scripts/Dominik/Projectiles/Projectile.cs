using Photon.Pun;
using UnityEngine;
using System;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    protected int damage;
    protected float moveSpeed;
    protected bool hitEntity;
    protected Transform target;

    [SerializeField] private string myPoolName;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;

    public event Action<Projectile> OnEntityHit = delegate { };
    public event Action<Projectile> OnEnvironmentHit = delegate { };

    public virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    public virtual void OnEnable()
    {
        hitEntity = false;
        Invoke("ReturnToPool", selfDestroyTime);
    }

    public virtual void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed;
    }

    public virtual void Fire(float speed, int damage, int targetID = 9999)
    {
        this.damage = damage;
        target = targetID != 9999 ? PhotonNetwork.GetPhotonView(targetID).transform : null;
        moveSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            HandleHitEntity(entity);
            return;
        }

        if (other.tag == "Environment")
        {
            HandleHitEnvironment();
            return;
        }
    }

    public virtual void HandleHitEntity(Entity entity)
    {
        if (photonView.IsMine)
        {
            entity.Hit(-damage);
            OnEntityHit(this);
        }

        if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, transform.position, Quaternion.identity);

            AOEDamage aoeComponent = newSpawn.GetComponent<AOEDamage>();
            if (aoeComponent)
            {
                aoeComponent.TriggerAOE(damage);
            }
        }

        hitEntity = true;
        ReturnToPool();
    }

    public virtual void HandleHitEnvironment()
    {
        OnEnvironmentHit(this);
        ReturnToPool();
    }

    // Seperate void so that i can Invoke it. Unity Invoke() doesnt support lambdas.
    protected void ReturnToPool()
    {
        if (!string.IsNullOrEmpty(myPoolName))
        {
            ObjectPooler.instance.AddToPool(myPoolName, gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public virtual void OnDisable()
    {
        rb.velocity = Vector3.zero;
        CancelInvoke();

        if (!string.IsNullOrEmpty(prefabToSpawnOnDeath))
        {
            ObjectPooler.instance.GrabFromPool(prefabToSpawnOnDeath, transform.position, Quaternion.identity);
        }
    }
}
