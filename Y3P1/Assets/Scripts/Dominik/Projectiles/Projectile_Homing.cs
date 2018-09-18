using UnityEngine;

public class Projectile_Homing : Projectile
{

    private Transform homingTarget;
    private float currentRotateSpeed;

    [SerializeField] private float rotateSpeed = 600;

    public override void OnEnable()
    {
        base.OnEnable();

        currentRotateSpeed = 0;
        homingTarget = GetClosestTarget();
        if (homingTarget)
        {
            ObjectPooler.instance.GrabFromPool("TargetMarkParticle", homingTarget.position + Vector3.up * 1, Quaternion.identity);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        if (homingTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(homingTarget.position + Vector3.up * 1 - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * currentRotateSpeed);
            if (currentRotateSpeed < rotateSpeed)
            {
                currentRotateSpeed += (Time.deltaTime * 800);
            }
        }
    }

    private Transform GetClosestTarget()
    {
        Transform bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        for (int i = 0; i < EntityManager.instance.aliveTargets.Count; i++)
        {
            // TODO: Maybe move PlayerController.mouseInWorldPos to somewhere else. This feels spaghetti-ish.
            Vector3 directionToTarget = EntityManager.instance.aliveTargets[i].transform.position - PlayerController.mouseInWorldPos;
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

        homingTarget = null;
    }
}
