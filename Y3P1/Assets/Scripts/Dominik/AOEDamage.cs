using UnityEngine;

public class AOEDamage : MonoBehaviour 
{

    private Collider[] entitiesInRange = new Collider[20];

    [SerializeField] private string myPoolName;
    [SerializeField] private float damageRange = 2;

    public void TriggerAOE(int damage)
    {
        int collidersFound = Physics.OverlapSphereNonAlloc(transform.position, damageRange, entitiesInRange);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = entitiesInRange[i].GetComponent<Entity>();
            if (entity)
            {
                if (entitiesInRange[i].transform.tag != "Player")
                {
                    entity.Hit(-damage);
                }
            }
        }
    }

    private void OnParticleSystemStopped()
    {
        ObjectPooler.instance.AddToPool(myPoolName, gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, damageRange);
    }
}
