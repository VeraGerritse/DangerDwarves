using UnityEngine;

public class AOEDamage : MonoBehaviour 
{

    private Collider[] entitiesInRange = new Collider[20];
    private float nextDamageTick;
    private int damage;
    private Projectile parentProjectile;

    [SerializeField] private string myPoolName;
    [SerializeField] private float damageRange = 2;
    [SerializeField] private bool continuousDamage;
    [SerializeField] private float damageInterval;
    [SerializeField] private bool initialiseInParent;

    private void Awake()
    {
        if (initialiseInParent)
        {
            parentProjectile = GetComponentInParent<Projectile>();
            parentProjectile.OnFire += ParentProjectile_OnFire;
        }
    }

    private void ParentProjectile_OnFire(Projectile obj)
    {
        Initialise(parentProjectile.damage);
    }

    public void Initialise(int damage)
    {
        this.damage = damage;

        TriggerAOE(damage);
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
        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, damageRange, entitiesInRange);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = entitiesInRange[i].GetComponent<Entity>();
            if (entity)
            {
                if (entitiesInRange[i].transform.parent.tag != "Player")
                {
                    entity.Hit(-damage);
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
