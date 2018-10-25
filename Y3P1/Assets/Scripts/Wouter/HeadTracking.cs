using UnityEngine;

public class HeadTracking : MonoBehaviour
{

    private bool initialised;

    public GameObject headbone;
    public Transform target;
    public Vector3 targetCords;
    public Quaternion mystuff;
    public float stopLookAtDistance = 1.5f;

    public bool gettarget;

    public void Initialise(bool local)
    {
        if (local)
        {
            initialised = true;
        }
    }

    private void LateUpdate()
    {
        if (!initialised)
        {
            return;
        }

        if (gettarget)
        {
            targetCords = PlayerController.mouseInWorldPos;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = targetCords - transform.position;

        Vector3 targetFixed = targetCords;
        targetFixed.y = headbone.transform.position.y;

        float dSqrToTarget = toOther.sqrMagnitude;
        if (dSqrToTarget > stopLookAtDistance || stopLookAtDistance == 0)
        {
            //headbone.transform.LookAt(targetFixed);

            if (Vector3.Dot(forward, toOther) > 0)
            {
                Vector3 relativePos = targetCords - transform.position;
                Quaternion rotation = Quaternion.LookRotation(relativePos);
                headbone.transform.rotation = rotation;
            }
            else
            {
                if (target)
                {
                    Vector3 relativePos = target.position - transform.position;
                    Quaternion rotation = Quaternion.LookRotation(relativePos);
                    headbone.transform.rotation = rotation;
                }
            }
        }
    }
}