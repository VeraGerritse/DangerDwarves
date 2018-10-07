using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviourPunCallbacks, IPunObservable
{

    private Entity target;
    private Entity targetEntity;
    private NavMeshAgent agent;
    private Entity entity;
    private Collider[] hits = new Collider[10];
    private AttackAnimation currentAttack;
    private Vector3 toTarget;
    private float randomRangedAttack;
    private List<AttackAnimation> rangedAttacks;
    private float nextRandomRangedTime;

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
    [SerializeField] [Range(0, 100)] private float randomRangedAttackChance;
    [SerializeField] private float randomRangedAttackInterval = 1f;

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
            Entity entity = initialChaseTrigger.eventCaller.GetComponentInChildren<Entity>();
            if (!entity.health.isDead)
            {
                photonView.RPC("SetTarget", RpcTarget.AllBuffered, entity.photonView.ViewID);
            }
        });

        GatherRangedAttacks();
    }

    private void Update()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

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
            if (target)
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                AggroClosestPlayer();
            }

            agent.isStopped = false;
            anim.SetBool("Is Walking", true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool("Is Walking", false);
        }

        if (target && rangedAttacks.Count != 0)
        {
            if (Time.time >= nextRandomRangedTime)
            {
                nextRandomRangedTime = Time.time + randomRangedAttackInterval;

                toTarget = target.transform.position - transform.position;

                float angle = Vector3.Angle(toTarget, transform.forward);
                if (angle < 20)
                {
                    randomRangedAttack = Random.Range(0, 101);
                    if (randomRangedAttack < randomRangedAttackChance)
                    {
                        StartRandomRangedAttack();
                    }
                }
            }
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

        if (target)
        {
            toTarget = target.transform.position - transform.position;
        }
        else
        {
            SetState(BehaviourState.Chase);
        }

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

    private void StartRandomRangedAttack()
    {
        canAttack = false;
        //canLookAtTarget = false;
        currentAttack = rangedAttacks[Random.Range(0, rangedAttacks.Count)];
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
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int collidersFound = Physics.OverlapSphereNonAlloc(damagePoint.position, damageRange, hits);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = hits[i].GetComponent<Entity>();
            if (entity)
            {
                if (entity.transform.parent.tag == "Player")
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
        if (!target && initialChaseTrigger.gameObject.activeInHierarchy)
        {
            AggroClosestPlayer();
        }

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

    private void AggroClosestPlayer()
    {
        Entity newTarget = EntityManager.instance.GetClosestPlayer(transform);
        if (newTarget)
        {
            photonView.RPC("SetTarget", RpcTarget.AllBuffered, newTarget.photonView.ViewID);
        }
    }

    [PunRPC]
    public void SetTarget(int targetID)
    {
        target = PhotonView.Find(targetID).GetComponent<Entity>();
        if (target)
        {
            target.OnDeath += Target_OnDeath;
            initialChaseTrigger.gameObject.SetActive(false);
            SetState(target ? BehaviourState.Chase : BehaviourState.Idle);
        }
    }

    private void Target_OnDeath()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            ResetAI();
            AggroClosestPlayer();
        }
    }

    private void SetState(BehaviourState state)
    {
        behaviourState = state;
    }

    private float GetDistanceToTarget()
    {
        Vector3 directionToTarget = target.transform.position - transform.position;
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

    private void GatherRangedAttacks()
    {
        rangedAttacks = new List<AttackAnimation>();
        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i].attackType == AttackAnimation.AttackType.Projectile)
            {
                rangedAttacks.Add(attacks[i]);
            }
        }
    }

    public override void OnDisable()
    {
        ResetAI();
    }

    private void ResetAI()
    {
        canAttack = true;
        canLookAtTarget = true;
        canMove = true;

        if (target)
        {
            target.OnDeath -= Target_OnDeath;
            target = null;
        }

        initialChaseTrigger.gameObject.SetActive(true);
        behaviourState = BehaviourState.Idle;
    }

    private bool TargetIsDead()
    {
        return targetEntity.health.isDead;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(canAttack);
            stream.SendNext(canLookAtTarget);
            stream.SendNext(canMove);
            stream.SendNext((int)behaviourState);
        }
        else
        {
            canAttack = (bool)stream.ReceiveNext();
            canLookAtTarget = (bool)stream.ReceiveNext();
            canMove = (bool)stream.ReceiveNext();
            behaviourState = (BehaviourState)stream.ReceiveNext();
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