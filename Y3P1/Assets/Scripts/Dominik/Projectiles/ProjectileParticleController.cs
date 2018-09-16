using UnityEngine;

public class ProjectileParticleController : MonoBehaviour 
{

    private Transform parent;

    private void Awake()
    {
        parent = GetComponentInParent<Projectile>().transform;
    }

    private void OnEnable()
    {
        transform.SetParent(null);
    }

    private void Update()
    {
        if (parent && !transform.parent)
        {
            transform.position = parent.position;
        }
    }

    private void OnParticleSystemStopped()
    {
        transform.SetParent(parent);
    }
}
