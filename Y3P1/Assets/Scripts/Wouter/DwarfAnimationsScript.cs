using UnityEngine;

public class DwarfAnimationsScript : MonoBehaviour
{

    public Animator myanim;

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 horizontal = transform.parent.right * x;
        Vector3 vertical = transform.parent.forward * y;

        Vector3 combinedAxis = horizontal + vertical;

        //horizontal = transform.parent.InverseTransformDirection(horizontal);
        //vertical = transform.parent.InverseTransformDirection(vertical);

        //NotificationManager.instance.NewNotification(horizontal.ToString() + "   " + vertical.ToString());

        myanim.SetFloat("HorizontalAxis", horizontal.x);
        myanim.SetFloat("VerticalAxis", vertical.z);
    }
}
