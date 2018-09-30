using System;
using UnityEngine;
using Y3P1;

public class PlayerController : MonoBehaviour
{

    public static Vector3 mouseInWorldPos;
    private GameObject mouseHitPlane;
    private Vector3 velocity;
    private Vector3 dodgeVelocity;
    private Quaternion dodgeRotation;
    private float nextDodgeTime;
    private bool isDodging;

    public Transform body;
    [SerializeField] private float moveSpeed;
    [SerializeField] private float dodgeSpeed;
    [SerializeField] private float dodgeCooldown = 1;
    [SerializeField] private float recoilImpactForce = 5f;
    [SerializeField] private LayerMask mouseHitPlaneLayermask;
    [SerializeField] private LayerMask heightCheckLayermask;

    public event Action<bool> OnDodge = delegate { };

    public void Initialise(bool local)
    {
        if (!local)
        {
            enabled = false;
            return;
        }

        CreateMouseHitPlane();
    }

    private void Update()
    {
        HandleRotation();
        HandleDodging();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    // Basic rigidbody movement using velocity.
    private void HandleMovement()
    {
        if (isDodging)
        {
            return;
        }

        // Get inputs.
        float x = Input.GetAxisRaw("Horizontal");
        float y = Input.GetAxisRaw("Vertical");

        // Multiply inputs by their corresponding axis.
        Vector3 horizontal = Vector3.right * x;
        Vector3 vertical = Vector3.forward * y;

        // Calculate normalized velocity and multiply it by the deltatime and movement speed.
        velocity = (horizontal + vertical).normalized * (Time.deltaTime * moveSpeed);

        // Use built in rigidbody function to move the player.
        // NOTE: MovePosition causes jittery movement! Setting the velocity directly and having interpolate on the rigidbody on works better.
        //Player.localPlayer.rb.MovePosition(transform.position + velocity);
        if (velocity != Vector3.zero)
        {
            // If the player is moving against a slope, help him a little bit by applying upwards velocity.
            RaycastHit hit;
            if (Physics.Raycast(transform.localPosition, velocity, out hit, 0.15f, heightCheckLayermask))
            {
                Player.localPlayer.rb.velocity = velocity + Vector3.up * 0.5f;
            }
            else
            {
                // If the player is moving on a flat surface, multiply velocity with a certain amount of downforce.
                if (Physics.Raycast(transform.localPosition, -transform.up, out hit, 0.05f, heightCheckLayermask))
                {
                    Player.localPlayer.rb.velocity = velocity + Vector3.down * 2;
                }
                // If the player is in the air and moving, increase the downwards velocity.
                else
                {
                    Player.localPlayer.rb.velocity = velocity + Vector3.down * 5;
                }

            }
        }
        else
        {
            // If the player is not moving and standing on a surface, zero his velocity so he doesnt slip.
            RaycastHit hit;
            if (Physics.Raycast(transform.localPosition, -transform.up, out hit, 0.05f, heightCheckLayermask))
            {
                Player.localPlayer.rb.velocity = Vector3.zero;
            }
            // If the player is not moving and not standing on a surface, apply a downwards velocity to get him back on the ground.
            else
            {
                Player.localPlayer.rb.velocity = Vector3.zero + Vector3.down * 5;
            }
        }

        Debug.DrawRay(transform.localPosition, velocity * 0.15f, Color.red);
    }

    // Gets the position of a raycast firing from the camera in the direction of the mouse and onto an invisible plane and uses that position for the player to rotate towards.
    private void HandleRotation()
    {
        if (isDodging)
        {
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(Player.localPlayer.playerCam.cameraComponent.ScreenPointToRay(Input.mousePosition), out hit, 50, mouseHitPlaneLayermask))
        {
            mouseInWorldPos = hit.point;
        }

        Vector3 lookAtTarget = new Vector3(mouseInWorldPos.x, body.position.y, mouseInWorldPos.z);
        body.LookAt(lookAtTarget);
    }

    // Adds a continuous force to the player and locks his movement and rotation abilities when dodging.
    private void HandleDodging()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (Time.time > nextDodgeTime)
            {
                if (velocity != Vector3.zero)
                {
                    StartDodge();
                    nextDodgeTime = Time.time + dodgeCooldown;
                }
            }
        }

        if (isDodging)
        {
            body.rotation = Quaternion.Slerp(body.rotation, dodgeRotation, Time.deltaTime * 10);
            Player.localPlayer.rb.AddForce(dodgeVelocity * dodgeSpeed, ForceMode.Force);
        }
    }

    private void StartDodge()
    {
        dodgeVelocity = velocity;
        dodgeRotation = Quaternion.LookRotation((body.position + dodgeVelocity) - body.position, Vector3.up);
        isDodging = true;
        OnDodge(true);
    }

    public void EndDodge()
    {
        isDodging = false;
        OnDodge(false);
    }

    // Creates an invisible plane for the mouse to raycast on to.
    private void CreateMouseHitPlane()
    {
        mouseHitPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        mouseHitPlane.layer = 11;
        mouseHitPlane.transform.SetParent(Player.localPlayer.transform);
        mouseHitPlane.transform.localPosition = new Vector3(0, 0.75f, 0);
        mouseHitPlane.transform.localScale = new Vector3(100, 1, 100);
        mouseHitPlane.GetComponent<MeshRenderer>().enabled = false;
        mouseHitPlane.GetComponent<MeshCollider>().convex = true;
    }
}
