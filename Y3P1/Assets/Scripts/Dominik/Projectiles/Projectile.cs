using Photon.Pun;
using System;
using UnityEngine;
using Y3P1;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    private Target defaultDamageTarget;
    protected bool hitAnything;
    private Transform owner;
    public enum Target { Enemy, Player };
    private Collider hitCollider;

    [SerializeField] private string myPoolName;
    public Target damageTarget;
    [SerializeField] private float selfDestroyTime = 5f;
    [SerializeField] private string prefabToSpawnOnHit;
    [SerializeField] private string prefabToSpawnOnDeath;
    [SerializeField] private bool stayOnOwner;
    [SerializeField] private bool isEnemyProjectile;

    [Header("Status Effect Module")]
    [SerializeField] private bool applyStatusEffect;
    [SerializeField] private StatusEffects.StatusEffectType effectToApply;
    [SerializeField] private float effectDuration;

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
        defaultDamageTarget = damageTarget;
    }

    public virtual void OnEnable()
    {
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
            hitCollider = entity.myCollider;

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
            hitCollider = other;
            HandleHitEnvironment();
            return;
        }
    }

    public virtual void HandleHitEntity(Entity entity)
    {
        if (fireData.ownerID == Player.localPlayer.photonView.ViewID)
        {
            entity.Hit(-fireData.damage, Stats.DamageType.Ranged, WeaponSlot.weaponBuffs);
            HandleProjectileStatusEffects(entity);
        }

        if (isEnemyProjectile)
        {
            HandleProjectileStatusEffects(entity);
        }

        OnEntityHit(this);

        SpawnPrefabOnHit();

        hitAnything = true;
        ReturnToPool();
    }

    public virtual void HandleHitEnvironment()
    {
        SpawnPrefabOnHit();
        OnEnvironmentHit(this);

        hitAnything = true;
        ReturnToPool();
    }

    private void HandleProjectileStatusEffects(Entity entity)
    {
        if (applyStatusEffect)
        {
            entity.photonView.RPC("SyncStatusEffects", RpcTarget.All, (int)effectToApply, effectDuration, fireData.damage);
        }
    }

    private void SpawnPrefabOnHit()
    {
        if (!string.IsNullOrEmpty(prefabToSpawnOnHit))
        {
            GameObject newSpawn = ObjectPooler.instance.GrabFromPool(prefabToSpawnOnHit, hitCollider.ClosestPoint(transform.position), Quaternion.identity);

            AOEEffect aoeComponent = newSpawn.GetComponent<AOEEffect>();
            if (aoeComponent)
            {
                aoeComponent.Initialise(fireData.damage);
            }
        }
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

    private void ResetProjectile()
    {
        rb.velocity = Vector3.zero;
        hitCollider = null;
        hitAnything = false;
        damageTarget = defaultDamageTarget;
        CancelInvoke();
    }

    public virtual void OnDisable()
    {
        ResetProjectile();

        if (!string.IsNullOrEmpty(prefabToSpawnOnDeath))
        {
            ObjectPooler.instance.GrabFromPool(prefabToSpawnOnDeath, transform.position, Quaternion.identity);
        }
    }
}