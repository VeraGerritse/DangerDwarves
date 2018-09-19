using UnityEngine;

public class Projectile_Homing : Projectile
{

    private float currentRotateSpeed;
    private bool hasMarkedTarget;

    [SerializeField] private float rotateSpeed = 600;

    public override void OnEnable()
    {
        base.OnEnable();

        currentRotateSpeed = 0;
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
                currentRotateSpeed += (Time.deltaTime * 800);
            }
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        target = null;
        hasMarkedTarget = false;
    }
}
