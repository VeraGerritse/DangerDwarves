using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracking : MonoBehaviour {

    public GameObject headbone;
    public Transform target;
    public Vector3 targetCords;
    public Quaternion mystuff;

    public bool gettarget;



    private void Start()
    {
        //target = PlayerController.mouseInWorldPos;

    }



    // Update is called once per frame
    void LateUpdate () {

        if (gettarget)
        {
            targetCords = PlayerController.mouseInWorldPos;
        }

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = targetCords - transform.position;

        
        if (Vector3.Dot(forward, toOther) > 0)
        {
            Vector3 relativePos = targetCords - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            headbone.transform.rotation = rotation;
        }
        else
        {
            /*
            Vector3 relativePos = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            headbone.transform.rotation = rotation;
            */
        }
        

    }
}
