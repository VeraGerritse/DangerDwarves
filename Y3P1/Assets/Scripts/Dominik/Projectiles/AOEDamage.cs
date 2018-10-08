using Photon.Pun;
using UnityEngine;

public class AOEDamage : MonoBehaviourPunCallbacks
{

    private Collider[] entitiesInRange = new Collider[30];
    private float nextDamageTick;
    private int damage;
    private Projectile parentProjectile;

    [SerializeField] private string myPoolName;
    [SerializeField] private float damageRange = 2;
    [SerializeField] private float damageMultiplier = 1;
    [SerializeField] private bool continuousDamage;
    [SerializeField] private float damageInterval;
    [SerializeField] private bool initialiseInParent;
    [SerializeField] private Projectile.Target damageTarget;

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
        if (continuousDamage)
        {
            nextDamageTick = Time.time + damageInterval;
        }
    }

    private void Update()
    {
        if (continuousDamage)
        {
            if (Time.time >= nextDamageTick)
            {
                nextDamageTick = Time.time + damageInterval;
                TriggerAOE(damage);
            }
        }
    }

    private void TriggerAOE(int damage)
    {
        if (!photonView.IsMine)
        {
            return;
        }

        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, damageRange, entitiesInRange);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = entitiesInRange[i].GetComponent<Entity>();
            if (entity)
            {
                switch (damageTarget)
                {
                    case Projectile.Target.Enemy:

                        if (entitiesInRange[i].transform.tag != "Player")
                        {
                            entity.Hit(-damage);
                        }
                        break;
                    case Projectile.Target.Player:

                        if (entitiesInRange[i].transform.tag == "Player")
                        {
                            entity.Hit(-damage);
                        }
                        break;
                }

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