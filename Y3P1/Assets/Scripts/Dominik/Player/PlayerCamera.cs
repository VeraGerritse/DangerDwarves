using UnityEngine;
using Y3P1;

public class PlayerCamera : MonoBehaviour 
{

    private Transform target;
    private Vector3 offset;
    private float fov;

    [HideInInspector] public Camera cameraComponent;

    [SerializeField] private bool lerp = true;
    [SerializeField] private float moveSpeed = 1f;
    public Transform camLookAtPoint;

    [Header("Zoom")]
    [SerializeField] private float zoomSensitivity;
    [SerializeField] private float zoomSpeed;
    [SerializeField] private float maxZoomIn;
    [SerializeField] private float maxZoomOut;

    private void Awake()
    {
        cameraComponent = GetComponentInChildren<Camera>();
        fov = cameraComponent.fieldOfView;
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

    private void Update()
    {
        ZoomCam();
    }

    private void LateUpdate()
    {
        if (target)
        {
            transform.position = lerp ? Vector3.Lerp(transform.position, target.position - offset, Time.deltaTime * moveSpeed) : target.position - offset;
        }
    }

    private void ZoomCam()
    {
        fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        fov = Mathf.Clamp(fov, maxZoomIn, maxZoomOut);

        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, fov, Time.deltaTime * zoomSpeed);
    }
}
