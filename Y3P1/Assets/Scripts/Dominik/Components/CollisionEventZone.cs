using UnityEngine;
using UnityEngine.Events;

public class CollisionEventZone : MonoBehaviour
{

    private enum CollisionType
    {
        Trigger,
        Collider
    }
    [SerializeField]
    private CollisionType collisionType;

    [SerializeField]
    private string lookForTag;

    public UnityEvent collisionEvent;
    [HideInInspector] public Transform eventCaller;

    private void OnTriggerEnter(Collider other)
    {
        if (collisionType != CollisionType.Trigger)
        {
            return;
        }

        if (other.tag == lookForTag)
        {
            eventCaller = other.transform;
            collisionEvent.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collisionType != CollisionType.Collider)
        {
            return;
        }

        if (collision.transform.tag == lookForTag)
        {
            eventCaller = collision.transform;
            collisionEvent.Invoke();
        }
    }
}
