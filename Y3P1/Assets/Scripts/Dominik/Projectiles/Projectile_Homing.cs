using UnityEngine;

public class Projectile_Homing : Projectile
{

    private Transform target;
    private bool hasMarkedTarget;
    private float currentRotateSpeed;

    // Max rotate speed.
    [SerializeField] private float rotateSpeed = 600;
    // This value determines how fast the rotate speed increases to its maximum.
    [SerializeField] private float rotateIncreaseSpeed = 800;

    public override void OnEnable()
    {
        base.OnEnable();

        currentRotateSpeed = 0;
        target = GetClosestTarget(fireData.mousePos);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (target)
        {
            if (!hasMarkedTarget)
            {
                hasMarkedTarget = true;
                ObjectPooler.instance.GrabFromPool("TargetMarkParticle", target.position + Vector3.up * 1, Quaternion.identity);
            }

            Quaternion targetRotation = Quaternion.LookRotation(target.position + Vector3.up * 1 - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * currentRotateSpeed);
            if (currentRotateSpeed < rotateSpeed)
            {
                currentRotateSpeed += (Time.deltaTime * rotateIncreaseSpeed);
            }
        }
    }

    private Transform GetClosestTarget(Vector3 origin)
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        for (int i = 0; i < EntityManager.instance.aliveTargets.Count; i++)
        {
            Vector3 directionToTarget = EntityManager.instance.aliveTargets[i].transform.position - origin;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = EntityManager.instance.aliveTargets[i].transform;
            }
        }

        return bestTarget;
    }

    public override void OnDisable()
    {
        base.OnDisable();

        target = null;
        hasMarkedTarget = false;
    }
}
