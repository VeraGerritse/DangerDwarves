using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ArmoryManager : MonoBehaviour 
{

    public static ArmoryManager instance;
    public static Item selectedItem;

    [SerializeField] private GameObject armoryCanvas;
    [SerializeField] private TextMeshProUGUI rerollSecondaryCostText;
    [SerializeField] private TextMeshProUGUI rerollStatsCostText;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }

        armoryCanvas.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (armoryCanvas.activeInHierarchy)
            {
                TogglePanel(armoryCanvas);
            }
        }
    }

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
        SelectItem(null);
    }

    private void SelectItem(Item item)
    {
        selectedItem = item;

        rerollSecondaryCostText.text = selectedItem != null ? "<color=yellow>" + CalculateRerollSecondaryCost().ToString() + "</color> Gold" : "No Weapon Selected";
        rerollStatsCostText.text = selectedItem != null ? "<color=yellow>" + CalculateRerollStatsCost().ToString() + "</color> Gold" : "No Equipment Selected";
    }

    private int CalculateRerollSecondaryCost()
    {
        return 0;
    }

    private int CalculateRerollStatsCost()
    {
        return 0;
    }

    public bool HasOpenUI()
    {
        return armoryCanvas.activeInHierarchy;
    }
}
