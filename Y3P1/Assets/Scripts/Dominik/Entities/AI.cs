using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviourPunCallbacks, IPunObservable
{

    private Transform target;
    private NavMeshAgent agent;
    private Entity entity;
    private Collider[] hits = new Collider[10];
    private AttackAnimation currentAttack;

    private bool canAttack = true;
    private bool canLookAtTarget = true;
    private bool canMove = true;

    public enum BehaviourState { Idle, Chase, Attack };
    public BehaviourState behaviourState;

    [SerializeField] private List<AttackAnimation> attacks = new List<AttackAnimation>();
    [SerializeField] private float attackDistance;
    [SerializeField] private float attackRangeLookAtSpeed;
    [SerializeField] private float damageRange;
    [SerializeField] private Transform damagePoint;
    [SerializeField] private int tempDamage = 10;

    [Space(10)]

    [SerializeField] private Animator anim;
    [SerializeField] private CollisionEventZone initialChaseTrigger;

    [System.Serializable]
    private struct AttackAnimation
    {
        public string attackName;
        [Range(0, 100)]
        public int attackChance;
        public enum AttackType { Animation, Projectile };
        public AttackType attackType;
        public string projectilePoolName;
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponentInChildren<Entity>();
        entity.OnHitEvent.AddListener(() => Hit());

        initialChaseTrigger.OnZoneEnterEvent.AddListener(() =>
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
        if (canMove)
        {
            agent.isStopped = false;
            agent.SetDestination(target.position);
            anim.SetBool("Is Walking", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("Is Walking", false);
        }

        if (GetDistanceToTarget() < attackDistance)
        {
            SetState(BehaviourState.Attack);
        }
    }

    private void HandleAttacking()
    {
        agent.isStopped = true;
        anim.SetBool("Is Walking", false);

        Vector3 toTarget = target.position - transform.position;

        if (canLookAtTarget)
        {
            Quaternion newRotation = Quaternion.LookRotation(toTarget);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * attackRangeLookAtSpeed);
        }

        if (canAttack)
        {
            if (GetDistanceToTarget() < attackDistance)
            {
                float angle = Vector3.Angle(toTarget, transform.forward);
                if (angle < 10)
                {
                    StartAttack();
                }
            }
            else
            {
                SetState(BehaviourState.Chase);
            }
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        canLookAtTarget = false;
        currentAttack = GetRandomAttack();
        anim.SetTrigger(currentAttack.attackName);
    }

    public void Attack()
    {
        if (currentAttack.attackType == AttackAnimation.AttackType.Animation)
        {
            AnimationAttack();
        }
        else if (currentAttack.attackType == AttackAnimation.AttackType.Projectile)
        {
            ProjectileAttack();
        }
    }

    private void AnimationAttack()
    {
        int collidersFound = Physics.OverlapSphereNonAlloc(damagePoint.position, damageRange, hits);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = hits[i].GetComponent<Entity>();
            if (entity)
            {
                if (hits[i].transform.parent.tag == "Player")
                {
                    entity.Hit(-tempDamage);
                }
            }
        }
    }

    private void ProjectileAttack()
    {
        photonView.RPC("SpawnProjectile", RpcTarget.All, currentAttack.projectilePoolName, 8, tempDamage);
    }

    [PunRPC]
    private void SpawnProjectile(string attackPoolName, int speed, int damage)
    {
        Projectile newProjectile = ObjectPooler.instance.GrabFromPool(attackPoolName, damagePoint.position, damagePoint.rotation).GetComponent<Projectile>();
        newProjectile.Fire(new Projectile.FireData
        {
            speed = speed,
            damage = damage
        });
    }

    public void EndAttack()
    {
        canAttack = true;
        canLookAtTarget = true;
    }

    private void Hit()
    {
        AggroClosestPlayerOnHit();

        if (behaviourState == BehaviourState.Attack)
        {
            anim.SetTrigger("Spider_hit");
        }
    }

    public void HitEnd()
    {
        canAttack = true;
        canLookAtTarget = true;
    }

    private void AggroClosestPlayerOnHit()
    {
        if (!target && initialChaseTrigger.gameObject.activeInHierarchy)
        {
            SetTarget(EntityManager.instance.GetClosestPlayer(transform));
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
        SetState(target ? BehaviourState.Chase : BehaviourState.Idle);
        photonView.RPC("SyncTarget", RpcTarget.AllBuffered, target.GetComponent<PhotonView>().ViewID);
    }

    [PunRPC]
    private void SyncTarget(int targetID)
    {
        target = PhotonView.Find(targetID).transform;
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

    private AttackAnimation GetRandomAttack()
    {
        int random = Random.Range(0, 101);

        for (int i = 0; i < attacks.Count; i++)
        {
            if (random < attacks[i].attackChance)
            {
                return attacks[i];
            }
        }

        return attacks[0];
    }

    public override void OnDisable()
    {
        canAttack = true;
        canLookAtTarget = true;
        canMove = true;

        target = null;
        initialChaseTrigger.gameObject.SetActive(true);
        behaviourState = BehaviourState.Idle;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //stream.SendNext(canAttack);
            //stream.SendNext(canLookAtTarget);
            //stream.SendNext(canMove);
            stream.SendNext(initialChaseTrigger.gameObject.activeSelf);
            //stream.SendNext((int)behaviourState);
        }
        else
        {
            //canAttack = (bool)stream.ReceiveNext();
            //canLookAtTarget = (bool)stream.ReceiveNext();
            //canMove = (bool)stream.ReceiveNext();
            initialChaseTrigger.gameObject.SetActive((bool)stream.ReceiveNext());
            //behaviourState = (BehaviourState)stream.ReceiveNext();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (damagePoint)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(damagePoint.position, damageRange);
        }
    }
}
