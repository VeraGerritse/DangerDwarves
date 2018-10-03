using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TEMPLightSwitcher : MonoBehaviour 
{

    private int index;

    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private List<PostProcessProfile> profiles = new List<PostProcessProfile>();

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            ChangeProfile();
        }
    }

    private void ChangeProfile()
    {
        if (index == profiles.Count - 1)
        {
            index = 0;
        }
        else
        {
            index++;
        }

        volume.profile = profiles[index];
    }
}
