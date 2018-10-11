using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering.PostProcessing;

public class Setting_Brightness : Setting 
{

    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private PostProcessVolume volume;
    [SerializeField] private List<PostProcessProfile> profiles = new List<PostProcessProfile>();

    public override void Awake()
    {
        base.Awake();

        dropdown.onValueChanged.AddListener((int i) => FindObjectOfType<PostProcessVolume>().profile = profiles[i]);
    }
}
