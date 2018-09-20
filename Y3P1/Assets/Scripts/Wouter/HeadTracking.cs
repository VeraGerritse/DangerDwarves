using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadTracking : MonoBehaviour {

    public GameObject headbone;
    public Transform target;
    public Quaternion mystuff;

    

    // Update is called once per frame
    void LateUpdate () {

        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 toOther = target.position - transform.position;


        if (Vector3.Dot(forward, toOther) > 0)
        {
            Vector3 relativePos = target.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(relativePos);
            headbone.transform.rotation = rotation;
        }

    }
}
