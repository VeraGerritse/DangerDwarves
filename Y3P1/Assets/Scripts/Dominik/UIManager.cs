using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour 
{

    public static UIManager instance;

    public static bool hasOpenUI;

    public Transform otherPlayersUISpawn;
    [SerializeField] private List<InventorySlot> hotbarSlots = new List<InventorySlot>();
    [SerializeField] private TextMeshProUGUI goldText;

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
    }
}
