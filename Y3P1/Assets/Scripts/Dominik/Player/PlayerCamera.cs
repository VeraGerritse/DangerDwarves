using UnityEngine;
using Y3P1;

public class PlayerCamera : MonoBehaviour
{

    private Transform target;
    private Vector3 offset;
    private float fov;
    private float defaultFOV;

    private float overrideFOV;
    private bool overridingFOV;
    private bool lockFOVOverride;

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
        defaultFOV = fov;
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

        Player.localPlayer.entity.OnDeath.AddListener(() => OverrideFOV(40, true));
        Player.localPlayer.entity.OnRevive.AddListener(() => OverrideFOV(defaultFOV, false));
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

    private void OverrideFOV(float newFOV, bool locked)
    {
        overrideFOV = newFOV;
        lockFOVOverride = locked;
        overridingFOV = true;
    }

    private void ZoomCam()
    {
        if (!UIManager.HasOpenUI)
        {
            fov -= Input.GetAxis("Mouse ScrollWheel") * zoomSensitivity;
        }
        fov = overridingFOV ? overrideFOV : fov;
        fov = Mathf.Clamp(fov, maxZoomIn, maxZoomOut);

        cameraComponent.fieldOfView = Mathf.Lerp(cameraComponent.fieldOfView, fov, Time.deltaTime * zoomSpeed);
        if (overridingFOV && cameraComponent.fieldOfView - overrideFOV < 0.1f)
        {
            if (!lockFOVOverride)
            {
                overridingFOV = false;
            }
        }
    }
}
