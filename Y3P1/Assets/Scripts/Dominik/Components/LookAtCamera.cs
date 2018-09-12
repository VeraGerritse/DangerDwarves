using UnityEngine;
using Y3P1;

public class LookAtCamera : MonoBehaviour 
{

    private Transform target;
    private Vector3 targetFixedPos;

    private void LateUpdate()
    {
        if (Player.localPlayerObject)
        {
            if (!target)
            {
                target = Player.localPlayerObject.GetComponent<Player>().playerCam.camLookAtPoint;
            }

            targetFixedPos = target.position;
            targetFixedPos.x = transform.position.x;

            transform.LookAt(targetFixedPos);
            transform.Rotate(0, 180, 0);
        }
    }
}
