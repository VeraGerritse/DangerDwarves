using Photon.Pun;
using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    protected bool hitEntity;
    private Transform owner;
    public enum Target { Enemy, Player };

    [SerializeField] private string myPoolName;
    public Target damageTarget;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;
    [SerializeField] private bool stayOnOwner;

    public event Action<Projectile> OnFire = delegate { };
    public event Action<Projectile> OnEntityHit = delegate { };
    public event Action<Projectile> OnEnvironmentHit = delegate { };

    public struct FireData
    {
        public float speed;
        public int damage;
        public Vector3 mousePos;
        public int ownerID;
    }
    public FireData fireData;

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

    private void Update()
    {
        if (stayOnOwner)
        {
            transform.position = owner.position;
        }
    }

    public virtual void FixedUpdate()
    {
        if (!stayOnOwner)
        {
            rb.velocity = transform.forward * fireData.speed;
        }
    }

    public virtual void Fire(FireData fireData)
    {
        this.fireData = fireData;
        owner = stayOnOwner ? PhotonNetwork.GetPhotonView(fireData.ownerID).transform : null;

        OnFire(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            switch (damageTarget)
            {
                case Target.Enemy:

                    if (entity.tag != "Player")
                    {
                        HandleHitEntity(entity);
                    }
                    break;
                case Target.Player:

                    if (entity.tag == "Player")
                    {
                        HandleHitEntity(entity);
                    }
                    break;
            }
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
            entity.Hit(-fireData.damage);
            OnEntityHit(this);
        }

        if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, transform.position, Quaternion.identity);

            AOEDamage aoeComponent = newSpawn.GetComponent<AOEDamage>();
            if (aoeComponent)
            {
                aoeComponent.Initialise(fireData.damage);
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