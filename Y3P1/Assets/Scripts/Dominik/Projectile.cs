using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    private Rigidbody rb;
    private PhotonView photonView;
    private int damage;
    private Transform homingTarget;
    private float moveSpeed;
    private float currentRotateSpeed;

    [SerializeField] private string myPoolName;
    [SerializeField] private bool isHoming;
    [SerializeField] private float rotateSpeed = 400;
    [SerializeField] private float selfDestroyTime = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
    }

    private void OnEnable()
    {
        Invoke("ReturnToPool", selfDestroyTime);

        if (isHoming)
        {
            currentRotateSpeed = 0;
            homingTarget = GetClosestTarget();
        }
    }

    private void FixedUpdate()
    {
        rb.velocity = transform.forward * moveSpeed;

        if (isHoming && homingTarget)
        {
            Quaternion targetRotation = Quaternion.LookRotation(homingTarget.position - transform.position);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * currentRotateSpeed);
            if (currentRotateSpeed < rotateSpeed)
            {
                currentRotateSpeed += (Time.deltaTime * 1000);
            }
        }
    }

    public void Fire(float speed, int damage)
    {
        this.damage = damage;
        moveSpeed = speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        Entity entity = other.GetComponent<Entity>();
        if (entity)
        {
            if (photonView.IsMine)
            {
                entity.Hit(-damage);
            }

            ReturnToPool();
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

    // Seperate void so that i can Invoke it. Unity Invoke() doesnt support lambdas.
    private void ReturnToPool()
    {
        ObjectPooler.instance.AddToPool(myPoolName, gameObject);
    }

    private void OnDisable()
    {
        rb.velocity = Vector3.zero;
        CancelInvoke();
    }
}
