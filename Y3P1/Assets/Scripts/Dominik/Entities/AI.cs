using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviourPunCallbacks, IPunObservable
{

    private Entity target;
    private Entity myEntity;
    private NavMeshAgent agent;
    private Collider[] hits = new Collider[10];
    private AISettings.AttackAnimation currentAttack;
    private Vector3 toTarget;
    private float randomRangedAttack;
    private List<AISettings.AttackAnimation> rangedAttacks;
    private float nextRandomRangedTime;
    private float currentIdleTime;

    private bool canAttack = true;
    private bool canLookAtTarget = true;
    private bool canMove = true;

    [SerializeField] private AISettings settings;

    public enum BehaviourState { Idle, Chase, Attack };
    [Space(10)]
    public BehaviourState behaviourState;

    [Space(10)]

    [SerializeField] private Transform damagePoint;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private CollisionEventZone initialChaseTrigger;
    [SerializeField] private Animator anim;

    private void Awake()
    {
        if (!settings)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.LogWarning("Destroyed entity because he had no AISettings assigned!");
                PhotonNetwork.Destroy(gameObject);
            }
        }

        agent = GetComponent<NavMeshAgent>();
        myEntity = GetComponentInChildren<Entity>();
        myEntity.OnHit.AddListener(() => Hit());
        myEntity.OnDeath.AddListener(() => Die());

        initialChaseTrigger.OnZoneEnterEvent.AddListener(() =>
        {
            Entity entity = initialChaseTrigger.eventCaller.GetComponentInChildren<Entity>();
            if (!entity.health.isDead)
            {
                photonView.RPC("SetTarget", RpcTarget.All, entity.photonView.ViewID);
            }
        });

        rangedAttacks = settings.GetAllRangedAttacks();
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
        if (myEntity.health.isDead)
        {
            return;
        }

        if (currentIdleTime > 0)
        {
            currentIdleTime -= Time.deltaTime;

            anim.SetBool(settings.walkAnimation, false);
            agent.isStopped = true;
        }
        else
        {
            if (Vector3.Distance(transform.position, agent.destination) < agent.stoppingDistance + 0.5f || agent.destination == transform.position)
            {
                float random = Random.value;
                if (random < 0.5f)
                {
                    agent.SetDestination(GetWanderDestination());
                    anim.SetBool(settings.walkAnimation, true);
                    agent.isStopped = false;
                }
                else
                {
                    currentIdleTime = Random.Range(settings.minIdleTime, settings.maxIdleTime);
                }
            }
        }
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
            anim.SetBool(settings.walkAnimation, true);
        }
        else
        {
            agent.isStopped = true;
            anim.SetBool(settings.walkAnimation, false);
        }

        RandomRangedAttack();

        if (GetDistanceToTarget() < settings.attackDistance)
        {
            SetState(BehaviourState.Attack);
        }
    }

    private void HandleAttacking()
    {
        agent.isStopped = true;
        anim.SetBool(settings.walkAnimation, false);

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
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * settings.attackRangeLookAtSpeed);
        }

        if (canAttack)
        {
            if (GetDistanceToTarget() < settings.attackDistance)
            {
                if (GetDistanceToTarget() < 1f)
                {
                    agent.SetDestination(new Vector3(target.transform.position.x + (Random.insideUnitCircle * 4).magnitude, 0, target.transform.position.z + (Random.insideUnitCircle * 4).magnitude));
                    agent.isStopped = false;
                    anim.SetBool(settings.walkAnimation, true);
                }
                else
                {
                    float angle = Vector3.Angle(toTarget, transform.forward);
                    if (angle < 10)
                    {
                        StartAttack();
                    }
                }
            }
            else
            {
                SetState(BehaviourState.Chase);
            }
        }
    }

    private void RandomRangedAttack()
    {
        if (target && rangedAttacks.Count != 0)
        {
            if (Time.time >= nextRandomRangedTime)
            {
                nextRandomRangedTime = Time.time + settings.randomRangedAttackInterval;

                toTarget = target.transform.position - transform.position;

                float angle = Vector3.Angle(toTarget, transform.forward);
                if (angle < 20)
                {
                    randomRangedAttack = Random.Range(0, 101);
                    if (randomRangedAttack < settings.randomRangedAttackChance)
                    {
                        StartRandomRangedAttack();
                    }
                }
            }
        }
    }

    private void StartAttack()
    {
        canAttack = false;
        canLookAtTarget = false;
        currentAttack = settings.GetRandomAttack();
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
        if (currentAttack.attackType == AISettings.AttackAnimation.AttackType.Animation)
        {
            AnimationAttack();
        }
        else if (currentAttack.attackType == AISettings.AttackAnimation.AttackType.Projectile)
        {
            ProjectileAttack();
        }

        if (settings.stopWhileAttacking)
        {
            agent.isStopped = true;
        }
    }

    private void AnimationAttack()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        int collidersFound = Physics.OverlapSphereNonAlloc(damagePoint.position, settings.damageRange, hits, settings.damageLayerMask);

        for (int i = 0; i < collidersFound; i++)
        {
            Entity entity = hits[i].GetComponent<Entity>();
            if (entity)
            {
                if (entity.transform.tag == "Player")
                {
                    entity.Hit(-myEntity.CalculateDamage(currentAttack.damageType), currentAttack.damageType);
                }
            }
        }
    }

    private void ProjectileAttack()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        ProjectileManager.instance.FireProjectile(new ProjectileManager.ProjectileData
        {
            spawnPosition = damagePoint.position,
            spawnRotation = damagePoint.rotation,
            projectilePool = currentAttack.projectilePoolName,
            speed = ProjectileManager.instance.GetProjectileSpeed(8, currentAttack.projectilePoolName),
            damage = myEntity.CalculateDamage(currentAttack.damageType)
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
            if (!string.IsNullOrEmpty(settings.hitAnimation))
            {
                anim.SetTrigger(settings.hitAnimation);
            }
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
            photonView.RPC("SetTarget", RpcTarget.All, newTarget.photonView.ViewID);
        }
    }

    [PunRPC]
    public void SetTarget(int targetID)
    {
        if (target)
        {
            return;
        }

        target = PhotonView.Find(targetID).GetComponent<Entity>();
        if (target)
        {
            target.OnDeath.AddListener(Target_OnDeath);
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

    private Vector3 GetWanderDestination()
    {
        Vector3 destination = Vector3.zero;
        int tries = 0;
        RaycastHit hit;

        while (destination == Vector3.zero)
        {
            tries++;
            if (tries > 25)
            {
                return transform.position;
            }

            Vector3 possibleDestination = (Random.insideUnitSphere * settings.wanderRadius) + transform.position;

            if (Physics.Raycast(possibleDestination, Vector3.down, out hit))
            {
                // If we hit the "Walkable" layer.
                if (hit.transform.gameObject.layer == 13)
                {
                    destination = possibleDestination;
                }
            }
        }

        return destination;
    }

    public override void OnDisable()
    {
        ResetAI();
    }

    private void ResetAI()
    {
        canAttack = true;
        canLookAtTarget = true;

        if (target)
        {
            target.OnDeath.AddListener(Target_OnDeath);
            target = null;
        }

        initialChaseTrigger.gameObject.SetActive(true);
        myEntity.gameObject.SetActive(true);
        agent.isStopped = false;
        healthBar.SetActive(true);
        currentIdleTime = 0;
        SetState(BehaviourState.Idle);
    }

    private bool TargetIsDead()
    {
        return target.health.isDead;
    }

    private void Die()
    {
        ResetAI();
        initialChaseTrigger.gameObject.SetActive(false);
        myEntity.gameObject.SetActive(false);
        agent.isStopped = true;
        anim.SetTrigger(settings.deathAnimation);
    }

    public void DisableHealthbar()
    {
        healthBar.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient && target)
        {
            photonView.RPC("SetTarget", RpcTarget.All, target.photonView.ViewID);
        }
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
            Gizmos.DrawWireSphere(damagePoint.position, settings.damageRange);
        }
    }
}