using TMPro;
using UnityEngine;
using Y3P1;

public class Setting_CamShake : Setting
{

    [SerializeField] private TMP_Dropdown dropdown;

    public override void Awake()
    {
        base.Awake();

        dropdown.onValueChanged.AddListener((int i) => Player.localPlayer.cameraShake.canShake = i == 0 ? true : false);
    }
}
