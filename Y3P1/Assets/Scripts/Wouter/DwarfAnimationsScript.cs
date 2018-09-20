using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DwarfAnimationsScript : MonoBehaviour {

    public Animator myanim;

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        myanim.SetFloat("HorizontalAxis", Input.GetAxis("Horizontal"));
        myanim.SetFloat("VerticalAxis", Input.GetAxis("Vertical"));
    }


}
