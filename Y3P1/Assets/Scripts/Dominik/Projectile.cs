using UnityEngine;

public class Projectile : MonoBehaviour
{

    //private bool hasBeenFired;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(float force)
    {
        rb.AddForce(transform.forward * force, ForceMode.Impulse);
        //hasBeenFired = true;
    }
}
