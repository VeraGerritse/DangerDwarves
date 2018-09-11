using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Setting_Resolution : Setting
{

    Resolution currentResolution;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    private Resolution[] availableResolutions;

    public override void Awake()
    {
        base.Awake();

        availableResolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().Reverse().ToArray();
        availableResolutions = availableResolutions.Where(resolution => resolution.width >= 800 && resolution.height >= 600).ToArray();
        List<string> resolutionDropdownOptions = new List<string>();

        currentResolution = Screen.currentResolution;
        int currentResolutionDropdownIndex = 0;

        for (int i = 0; i < availableResolutions.Length; i++)
        {
            resolutionDropdownOptions.Add(availableResolutions[i].width + "x" + availableResolutions[i].height);
            if (availableResolutions[i].width == currentResolution.width && availableResolutions[i].height == currentResolution.height)
            {
                currentResolutionDropdownIndex = i;
            }
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(resolutionDropdownOptions);
        resolutionDropdown.value = currentResolutionDropdownIndex;
        resolutionDropdown.onValueChanged.AddListener(ChangeSetting);
    }

    private void ChangeSetting(int setting)
    {
        Resolution newResolution = availableResolutions[setting];

        if (newResolution.width != currentResolution.width && newResolution.height != currentResolution.height)
        {
            Screen.SetResolution(newResolution.width, newResolution.height, Screen.fullScreenMode);
            currentResolution = newResolution;
        }
    }
}
