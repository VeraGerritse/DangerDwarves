using Photon.Pun;
using UnityEngine;
using Y3P1;

public class AOEEffect : MonoBehaviourPunCallbacks
{

    private Collider[] entitiesInRange = new Collider[30];
    private float nextDamageTick;
    private int damage;
    private Projectile parentProjectile;

    [SerializeField] private string myPoolName;

    [Header("General Settings")]
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private Projectile.Target damageTarget;
    [SerializeField] private bool initialiseInParent;
    [SerializeField] private bool continuousEffect;
    [SerializeField] private float effectInterval;

    [Header("Damage Module")]
    [SerializeField] private float damageRange = 2;
    [SerializeField] private float damageMultiplier = 1;

    [Header("Status Effect Module")]
    [SerializeField] private bool applyStatusEffect;
    [SerializeField] private StatusEffects.StatusEffectType effectToApply;
    [SerializeField] private float effectDuration;

    [Header("Knockback Module")]
    [SerializeField] private bool pushToCenter;
    [SerializeField] private float pushToCenterForce;

    private void Awake()
    {
        // This means that this object is childed to a Projectile so that it can receive that projectiles data when it gets fired.
        if (initialiseInParent)
        {
            parentProjectile = GetComponentInParent<Projectile>();
            parentProjectile.OnFire += ParentProjectile_OnFire;
        }
    }

    private void ParentProjectile_OnFire(Projectile obj)
    {
        Initialise(parentProjectile.fireData.damage);
    }

    public void Initialise(int damage)
    {
        this.damage = Mathf.RoundToInt(damage * damageMultiplier);
        damageTarget = parentProjectile ? parentProjectile.damageTarget : damageTarget;

        TriggerAOE(this.damage);
        if (continuousEffect)
        {
            nextDamageTick = Time.time + effectInterval;
        }
    }

    private void Update()
    {
        if (continuousEffect)
        {
            if (Time.time >= nextDamageTick)
            {
                nextDamageTick = Time.time + effectInterval;
                TriggerAOE(damage);
            }
        }
    }

    private void TriggerAOE(int damage)
    {
        TriggerCameraShake();

        if (!photonView.IsMine)
        {
            return;
        }

        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, damageRange, entitiesInRange, hitLayerMask);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = entitiesInRange[i].GetComponent<Entity>();
            if (entity)
            {
                DamageModule(entity);
                StatusEffectModule(entity);
                KnockbackModule(entity);
            }
        }
    }

    private void TriggerCameraShake()
    {
        if (!continuousEffect)
        {
            Vector3 viewPos = Player.localPlayer.playerCam.cameraComponent.WorldToViewportPoint(transform.position);
            if (viewPos.x >= 0 && viewPos.x <= 1 && viewPos.y >= 0 && viewPos.y <= 1)
            {
                Player.localPlayer.cameraShake.Trauma = 1f;
            }
        }
    }

    private void DamageModule(Entity entity)
    {
        switch (damageTarget)
        {
            case Projectile.Target.Enemy:

                if (entity.transform.tag != "Player")
                {
                    entity.Hit(-damage, Stats.DamageType.AOE);
                }
                break;
            case Projectile.Target.Player:

                if (entity.transform.tag == "Player")
                {
                    entity.Hit(-damage, Stats.DamageType.AOE);
                }
                break;
        }
    }

    private void KnockbackModule(Entity entity)
    {
        if (pushToCenter)
        {
            entity.KnockBack(transform.position - entity.transform.position, pushToCenterForce);
        }
    }

    private void StatusEffectModule(Entity entity)
    {
        if (applyStatusEffect)
        {
            switch (damageTarget)
            {
                case Projectile.Target.Enemy:

                    if (entity.transform.tag != "Player")
                    {
                        entity.photonView.RPC("SyncStatusEffects", RpcTarget.All, (int)effectToApply, effectDuration, damage);
                    }
                    break;
                case Projectile.Target.Player:

                    if (entity.transform.tag == "Player")
                    {
                        entity.photonView.RPC("SyncStatusEffects", RpcTarget.All, (int)effectToApply, effectDuration, damage);
                    }
                    break;
            }
        }
    }

    private void OnParticleSystemStopped()
    {
        if (!string.IsNullOrEmpty(myPoolName))
        {
            ObjectPooler.instance.AddToPool(myPoolName, gameObject);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}