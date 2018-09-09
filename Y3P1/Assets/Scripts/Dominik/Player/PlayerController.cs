using UnityEngine;
using Y3P1;

public class PlayerController : MonoBehaviour
{

    [SerializeField] private float moveSpeed;
    [SerializeField] Transform body;

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
        Player.localPlayer.rb.MovePosition(transform.position + velocity);
    }

    // Gets the position of a raycast firing from the camera to the mouse with a certain distance and uses that position for the player to rotate towards.
    private void HandleRotation()
    {
        Ray ray = Player.localPlayer.playerCam.cameraComponent.ScreenPointToRay(Input.mousePosition);
        Vector3 mouseInWorldPos = ray.GetPoint((transform.position - ray.origin).magnitude * 0.95f);

        Vector3 lookAtTarget = new Vector3(mouseInWorldPos.x, body.position.y, mouseInWorldPos.z);
        body.LookAt(lookAtTarget);
    }
}
