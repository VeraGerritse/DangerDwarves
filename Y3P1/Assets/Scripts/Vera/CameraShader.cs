using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShader : MonoBehaviour {
    [SerializeField] PlayerAppearance playerAppearance;
    [SerializeField] Material eyes;
    [SerializeField] Shader s1;
    [SerializeField] Shader s2;

    private void OnPreRender()
    {
        playerAppearance.dwarfRenderer.material.shader = s1;
        eyes.shader = s1;
    }

    private void OnPostRender()
    {
        playerAppearance.dwarfRenderer.material.shader = s2;
        eyes.shader = s2;
    }
}
