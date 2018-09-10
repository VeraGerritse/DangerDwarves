using UnityEngine;
using Y3P1;

public class LookAtCamera : MonoBehaviour 
{

    private Transform target;

    private void LateUpdate()
    {
        if (Player.localPlayerObject)
        {
            if (!target)
            {
                target = Player.localPlayerObject.GetComponent<Player>().playerCam.transform;
            }

            transform.LookAt(target);
            transform.Rotate(0, 180, 0);
        }
    }
}
