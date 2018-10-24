using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ArmoryManager : MonoBehaviour
{

    public static ArmoryManager instance;
    public static Item selectedItem;
    public static StatSelectButton.StatToSelect selectedStat;

    private enum ServiceType { RerollSecondary, RerollStats };

    [SerializeField] private GameObject armoryCanvas;
    [SerializeField] private GameObject hoverSlotCanvas;

    [Header("Reroll Secondary")]
    [SerializeField] private GameObject rerollSecondaryPanel;
    [SerializeField] private TextMeshProUGUI rerollSecondaryCostText;
    [SerializeField] private Image rerollSecondarySlotItemImage;
    [SerializeField] private Image rerollSecondarySlotItemRarityImage;
    [SerializeField] private TextMeshProUGUI currentSecondaryText;
    [SerializeField] private Animator rerollSecondaryAnim;

    [Header("Reroll Stats")]
    [SerializeField] private GameObject rerollStatsPanel;
    [SerializeField] private TextMeshProUGUI rerollStatsCostText;
    [SerializeField] private Image rerollStatsSlotItemImage;
    [SerializeField] private Image rerollStatsSlotItemRarityImage;
    [SerializeField] private TextMeshProUGUI currentStatsText;
    [SerializeField] private Animator rerollStatsAnim;
    [SerializeField] private List<StatSelectButton> statSelectButtons = new List<StatSelectButton>();

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

    private void Start()
    {
        Player.localPlayer.myInventory.OnRightClickInventorySlot += (i) => SelectItem(i);
        Player.localPlayer.myInventory.OnHoverInventorySlot += ToggleHoverSlotCanvas;
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

        if (hoverSlotCanvas.activeInHierarchy)
        {
            hoverSlotCanvas.transform.position = Input.mousePosition;
        }
    }

    public void TogglePanel(GameObject panel)
    {
        panel.SetActive(!panel.activeInHierarchy);
        SelectItem(null);
    }

    private void ToggleHoverSlotCanvas(Item item)
    {
        if (item != null)
        {
            if (rerollSecondaryPanel.activeInHierarchy)
            {
                if (item is Weapon)
                {
                    hoverSlotCanvas.SetActive(true);
                    return;
                }
            }
            else
            {
                if (item is Weapon || item is Helmet || item is Trinket)
                {
                    hoverSlotCanvas.SetActive(true);
                    return;
                }
            }
        }

        hoverSlotCanvas.SetActive(false);
    }

    private void SelectItem(Item item)
    {
        if (item == null)
        {
            ResetSelectedItemSlots();
        }

        if (rerollSecondaryPanel.activeInHierarchy)
        {
            if (item is Weapon)
            {
                selectedItem = item;
                SetupSelectedItemSlot(ServiceType.RerollSecondary, rerollSecondaryCostText, rerollSecondarySlotItemImage, rerollSecondarySlotItemRarityImage);
            }
        }

        if (rerollStatsPanel.activeInHierarchy)
        {
            if (item is Weapon || item is Helmet || item is Trinket)
            {
                selectedItem = item;
                SetupSelectedItemSlot(ServiceType.RerollStats, rerollStatsCostText, rerollStatsSlotItemImage, rerollStatsSlotItemRarityImage);
            }
        }
    }

    private void SetupSelectedItemSlot(ServiceType serviceType, TextMeshProUGUI rerollCostText, Image slotImage, Image rarityImage)
    {
        rerollCostText.text = serviceType == ServiceType.RerollSecondary ? "<color=yellow>" + CalculateRerollSecondaryCost().ToString() + "</color> Gold" :
                                                                           "<color=yellow>" + CalculateRerollStatsCost().ToString() + "</color> Gold";

        slotImage.sprite = Database.hostInstance.allSprites[selectedItem.spriteIndex];
        slotImage.enabled = true;

        rarityImage.color = Player.localPlayer.myInventory.GetSlotColor(selectedItem.itemRarity);
        rarityImage.enabled = true;

        if (serviceType == ServiceType.RerollSecondary)
        {
            string[] projectileName = (selectedItem as Weapon).secondaryProjectile.Split('_');
            currentSecondaryText.text = projectileName[1];
        }
        else
        {
            currentStatsText.text = "Stamina: <color=yellow>" + selectedItem.myStats.stamina + "</color>\n" +
                                    "Stength: <color=yellow>" + selectedItem.myStats.strength + "</color>\n" +
                                    "Agility: <color=yellow>" + selectedItem.myStats.agility + "</color>\n" +
                                    "Willpower: <color=yellow>" + selectedItem.myStats.willpower + "</color>\n" +
                                    "Defense: <color=yellow>" + selectedItem.myStats.defense + "</color>";
        }
    }

    public void SelectStat(StatSelectButton statButton)
    {
        for (int i = 0; i < statSelectButtons.Count; i++)
        {
            if (statSelectButtons[i].statToSelect == statButton.statToSelect)
            {
                statSelectButtons[i].SelectStat();
            }
            else
            {
                statSelectButtons[i].Deselect();
            }
        }
    }

    public void RerollSecondaryButton()
    {
        if (selectedItem != null && Player.localPlayer.myInventory.totalGoldAmount >= CalculateRerollSecondaryCost())
        {
            rerollSecondaryAnim.SetTrigger("RerollSecondary");

            if (selectedItem is Weapon_Melee)
            {
                (selectedItem as Weapon).secondaryProjectile = Database.hostInstance.GetMeleeSecundary(selectedItem.itemRarity == Item.ItemRarity.legendary);
            }
            else if (selectedItem is Weapon_Ranged)
            {
                (selectedItem as Weapon).secondaryProjectile = Database.hostInstance.GetRangedSecundary(selectedItem.itemRarity == Item.ItemRarity.legendary);
            }

            Player.localPlayer.myInventory.UpdateGold(-CalculateRerollSecondaryCost());
            SelectItem(selectedItem);
        }
    }

    public void RerollStatsButton()
    {
        if (selectedItem != null && Player.localPlayer.myInventory.totalGoldAmount >= CalculateRerollStatsCost())
        {
            rerollStatsAnim.SetTrigger("RerollStats");

            Stats newStats = LootRandomizer.instance.NewStats(selectedItem.itemLevel);
            if (selectedStat != StatSelectButton.StatToSelect.Nothing)
            {
                newStats.stamina = selectedStat == StatSelectButton.StatToSelect.Stamina ? Mathf.RoundToInt(1.5f * newStats.stamina) : Mathf.RoundToInt(0.9f * newStats.stamina);
                newStats.strength = selectedStat == StatSelectButton.StatToSelect.Strength ? Mathf.RoundToInt(1.5f * newStats.strength) : Mathf.RoundToInt(0.9f * newStats.strength);
                newStats.agility = selectedStat == StatSelectButton.StatToSelect.Agility ? Mathf.RoundToInt(1.5f * newStats.agility) : Mathf.RoundToInt(0.9f * newStats.agility);
                newStats.willpower = selectedStat == StatSelectButton.StatToSelect.Willpower ? Mathf.RoundToInt(1.5f * newStats.willpower) : Mathf.RoundToInt(0.9f * newStats.willpower);
                newStats.defense = selectedStat == StatSelectButton.StatToSelect.Defense ? Mathf.RoundToInt(1.5f * newStats.defense) : Mathf.RoundToInt(0.9f * newStats.defense);
            }
            selectedItem.myStats = newStats;

            Player.localPlayer.myInventory.UpdateGold(-CalculateRerollStatsCost());
            SelectItem(selectedItem);
        }
    }

    private void ResetSelectedItemSlots()
    {
        rerollSecondaryCostText.text = "No Weapon Selected";
        rerollStatsCostText.text = "No Equipment Selected";
        rerollSecondarySlotItemImage.enabled = false;
        rerollStatsSlotItemImage.enabled = false;
        rerollSecondarySlotItemRarityImage.enabled = false;
        rerollStatsSlotItemRarityImage.enabled = false;

        currentSecondaryText.text = "";
        currentStatsText.text = "Stamina:\n" +
                                "Stength:\n" +
                                "Agility:\n" +
                                "Willpower:\n" +
                                "Defense:";
        SelectStat(new StatSelectButton { statToSelect = StatSelectButton.StatToSelect.Nothing });
    }

    private int CalculateRerollSecondaryCost()
    {
        return selectedItem != null ? selectedItem.itemLevel * 8000 : 0;
    }

    private int CalculateRerollStatsCost()
    {
        return selectedItem != null ? selectedItem.itemLevel * 2000 : 0;
    }

    public bool HasOpenUI()
    {
        return armoryCanvas.activeInHierarchy;
    }
}
