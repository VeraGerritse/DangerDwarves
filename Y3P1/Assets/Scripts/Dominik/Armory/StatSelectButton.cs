using UnityEngine;
using UnityEngine.UI;

public class StatSelectButton : MonoBehaviour
{

    private Image selectedOverlay;

    public enum StatToSelect { Nothing, Stamina, Strength, Agility, Willpower, Defense };
    public StatToSelect statToSelect;

    private void Awake()
    {
        selectedOverlay = transform.GetChild(0).GetComponent<Image>();
    }

    public void SelectStat()
    {
        if (selectedOverlay.enabled)
        {
            Deselect();
            ArmoryManager.selectedStat = StatToSelect.Nothing;
        }
        else
        {
            selectedOverlay.enabled = true;
            ArmoryManager.selectedStat = statToSelect;
        }
    }

    public void Deselect()
    {
        if (selectedOverlay)
        {
            selectedOverlay.enabled = false;
        }
    }
}
