using UnityEngine;
using Y3P1;

public class HeadTracking : MonoBehaviour
{

    private bool initialised;

    public GameObject headbone;
    public Transform target;
    public Vector3 targetCords;
    public Quaternion mystuff;

    public bool gettarget;

    private void Awake()
    {
        Player.OnLocalPlayerInitialise += Initialise;
    }

    private void Initialise()
    {
        initialised = true;
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
        if (dSqrToTarget > 1.5f)
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

    private void OnDisable()
    {
        Player.OnLocalPlayerInitialise -= Initialise;
    }
}
