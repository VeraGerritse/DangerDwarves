using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class ArmoryManager : MonoBehaviour
{

    public static ArmoryManager instance;
    public static Item selectedItem;

    private enum ServiceType { RerollSecondary, RerollStats };

    [SerializeField] private GameObject armoryCanvas;

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
            currentSecondaryText.text = (selectedItem as Weapon).secondaryProjectile;
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
            selectedItem.myStats = LootRandomizer.instance.NewStats(selectedItem.itemLevel);
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
    }

    private int CalculateRerollSecondaryCost()
    {
        return selectedItem != null ? selectedItem.itemLevel * 10000 : 0;
    }

    private int CalculateRerollStatsCost()
    {
        return selectedItem != null ? selectedItem.itemLevel * 3000 : 0;
    }

    public bool HasOpenUI()
    {
        return armoryCanvas.activeInHierarchy;
    }
}
