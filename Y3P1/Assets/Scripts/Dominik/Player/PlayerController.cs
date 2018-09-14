using UnityEngine;
using Y3P1;

public class PlayerController : MonoBehaviour
{

    private GameObject mouseHitPlane;
    private Vector3 mouseInWorldPos;

    [SerializeField] private float moveSpeed;
    [SerializeField] Transform body;
    [SerializeField] LayerMask mouseHitPlaneLayermask;

    public void Initialise()
    {
        CreateMouseHitPlane();
    }

    private void Update()
    {
        HandleRotation();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // Basic rigidbody movement using velocity.
    private void HandleMovement()
    {
        // Get inputs.
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        // Multiply inputs by their corresponding axis.
        Vector3 horizontal = Vector3.right * x;
        Vector3 vertical = Vector3.forward * y;

        // Calculate normalized velocity and multiply it by the deltatime and movement speed.
        Vector3 velocity = (horizontal + vertical).normalized * (Time.deltaTime * moveSpeed);

        // Use built in rigidbody function to move the player.
        // NOTE: MovePosition causes jittery movement! Setting the velocity directly and having interpolate on the rigidbody on works better.
        //Player.localPlayer.rb.MovePosition(transform.position + velocity);
        Player.localPlayer.rb.velocity = velocity;
    }

    // Gets the position of a raycast firing from the camera in the direction of the mouse and onto an invisible plane and uses that position for the player to rotate towards.
    private void HandleRotation()
    {
        //Ray ray = Player.localPlayer.playerCam.cameraComponent.ScreenPointToRay(Input.mousePosition);
        //Vector3 mouseInWorldPos = ray.GetPoint((transform.position - ray.origin).magnitude * 1.1f);

        RaycastHit hit;
        if (Physics.Raycast(Player.localPlayer.playerCam.cameraComponent.ScreenPointToRay(Input.mousePosition), out hit, 50, mouseHitPlaneLayermask))
        {
            mouseInWorldPos = hit.point;
        }

        Vector3 lookAtTarget = new Vector3(mouseInWorldPos.x, body.position.y, mouseInWorldPos.z);
        body.LookAt(lookAtTarget);
    }

    // Creates an invisible plane for the mouse to raycast on to.
    private void CreateMouseHitPlane()
    {
        mouseHitPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mouseHitPlane.layer = 11;
        mouseHitPlane.transform.SetParent(Player.localPlayer.transform);
        mouseHitPlane.transform.localPosition = Vector3.zero;
        mouseHitPlane.transform.localScale = new Vector3(100, 1, 100);
        mouseHitPlane.GetComponent<MeshRenderer>().enabled = false;
        mouseHitPlane.GetComponent<MeshCollider>().convex = true;
    }
}
