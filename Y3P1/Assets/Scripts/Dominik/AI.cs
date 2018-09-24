using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class AI : MonoBehaviourPunCallbacks
{

    private Animator anim;
    private Transform target;
    private NavMeshAgent agent;
    private Entity entity;

    public enum BehaviourState { Idle, Chase, Attack };
    public BehaviourState behaviourState;

    private void Awake()
    {
        //anim = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        entity = GetComponent<Entity>();
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
        // Set animator state to idle.
    }

    private void HandleChasing()
    {
        // Set animator state to walk.
        agent.SetDestination(target.position);
    }

    private void HandleAttacking()
    {
        // Set animator state to attack.
    }

    private void SetTarget(Transform target)
    {
        this.target = target;
        SetState(target ? BehaviourState.Chase : BehaviourState.Idle);
    }

    private void SetState(BehaviourState state)
    {
        behaviourState = state;
    }
}
