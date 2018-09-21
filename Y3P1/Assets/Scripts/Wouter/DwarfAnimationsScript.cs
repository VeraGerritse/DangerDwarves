using UnityEngine;

public class DwarfAnimationsScript : MonoBehaviour
{

    public Animator myanim;

    private void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 combinedAxis = new Vector3(x, 0, y);
        combinedAxis = transform.parent.InverseTransformDirection(combinedAxis);

        myanim.SetFloat("HorizontalAxis", combinedAxis.x);
        myanim.SetFloat("VerticalAxis", combinedAxis.z);
    }
}
