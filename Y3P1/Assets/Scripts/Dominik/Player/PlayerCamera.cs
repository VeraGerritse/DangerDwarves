using UnityEngine;
using Y3P1;

public class PlayerCamera : MonoBehaviour 
{

    private Transform target;
    private Vector3 offset;

    [HideInInspector] public Camera cameraComponent;

    [SerializeField] private bool lerp = true;
    [SerializeField] private float moveSpeed = 1f;
    public Transform camLookAtPoint;

    private void Awake()
    {
        cameraComponent = GetComponentInChildren<Camera>();
    }

    public void Initialise(bool local)
    {
        if (!local)
        {
            gameObject.SetActive(false);
            return;
        }

        target = Player.localPlayerObject.transform;
        offset = target.position - transform.position;

        transform.SetParent(lerp ? null : transform);
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = lerp ? Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * moveSpeed) : target.position - offset;
        }
    }
}
