using Photon.Pun;
using UnityEngine;
using System;
using Y3P1;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    protected float moveSpeed;
    protected bool hitEntity;
    protected Transform target;
    [HideInInspector] public int damage;

    [SerializeField] private string myPoolName;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;
    [SerializeField] private bool stayOnPlayer;

    public event Action<Projectile> OnFire = delegate { };
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

    private void Update()
    {
        if (stayOnPlayer)
        {
            transform.position = Player.localPlayer.transform.position;
        }
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

        OnFire(this);
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
                aoeComponent.Initialise(damage);
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
