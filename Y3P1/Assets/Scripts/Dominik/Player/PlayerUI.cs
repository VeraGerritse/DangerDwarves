using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Y3P1;

public class PlayerUI : MonoBehaviour 
{

    private bool isInitialised;
    private Player target;

    [SerializeField] private TextMeshProUGUI nameText;

    private void Update()
    {
        if (!target && isInitialised)
        {
            Destroy(gameObject);
        }
    }

    public void Initialise(Player target)
    {
        if (!target)
        {
            return;
        }

        this.target = target;
        if (nameText)
        {
            nameText.text = target.photonView.Owner.NickName;
        }

        GetComponentInChildren<HealthBar>().Initialise(target.health);

        isInitialised = true;
    }
}
