using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviourPunCallbacks, IPunObservable
{

    private Transform target;
    private NavMeshAgent agent;
    private Entity entity;

    private bool canAttack = true;
    private bool canLookAtTarget = true;
    private bool isInAttackAnim;

    public enum BehaviourState { Idle, Chase, Attack };
    public BehaviourState behaviourState;

    [SerializeField] private float attackDistance;
    [SerializeField] private float attackRangeLookAtSpeed;

    [Space(10)]

    [SerializeField] private Animator anim;
    [SerializeField] private CollisionEventZone initialChaseTrigger;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponentInChildren<Entity>();
        entity.OnHitEvent.AddListener(() =>
        {
            AggroClosestPlayerOnHit();
            anim.SetTrigger("Hit");
        });

        initialChaseTrigger.OnCollisionEvent.AddListener(() =>
        {
            SetTarget(initialChaseTrigger.eventCaller);
            initialChaseTrigger.gameObject.SetActive(false);
        });
    }

    private void Update()
    {
        switch (behaviourState)
        {
            case BehaviourState.Idle:

                HandleIdling();
                break;
            case BehaviourState.Chase:

                HandleChasing();
                break;
            case BehaviourState.Attack:

                HandleAttacking();
                break;
        }
    }

    private void HandleIdling()
    {
        anim.SetBool("Is Walking", false);
    }

    private void HandleChasing()
    {
        anim.SetBool("Is Walking", true);
        agent.SetDestination(target.position);

        if (GetDistanceToTarget() < attackDistance)
        {
            SetState(BehaviourState.Attack);
        }
    }

    private void HandleAttacking()
    {
        anim.SetBool("Is Walking", false);

        if (canLookAtTarget)
        {
            Vector3 toTarget = target.position - transform.position;
            Quaternion newRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * attackRangeLookAtSpeed);
        }

        if (canAttack)
        {
            Attack();
        }

        if (GetDistanceToTarget() > attackDistance)
        {
            //if (canAttack)
            //{
                SetState(BehaviourState.Chase);
            //}
        }
    }

    private void Attack()
    {
        canAttack = false;
        //canLookAtTarget = false;
        anim.SetTrigger("Attacck");
    }

    public void EndAttack()
    {
        canAttack = true;
        canLookAtTarget = true;
    }

    private void AggroClosestPlayerOnHit()
    {
        if (!target)
        {
            SetTarget(EntityManager.instance.GetClosestPlayer(transform));
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        SetState(target ? BehaviourState.Chase : BehaviourState.Idle);
    }

    private void SetState(BehaviourState state)
    {
        behaviourState = state;
    }

    private float GetDistanceToTarget()
    {
        Vector3 directionToTarget = target.position - transform.position;
        return directionToTarget.sqrMagnitude;
    }

    public override void OnDisable()
    {
        target = null;
        initialChaseTrigger.gameObject.SetActive(true);
        behaviourState = BehaviourState.Idle;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(canAttack);
            stream.SendNext(canLookAtTarget);
        }
        else
        {
            canAttack = (bool)stream.ReceiveNext();
            canLookAtTarget = (bool)stream.ReceiveNext();
        }
    }
}
