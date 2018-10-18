using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class UIManager : MonoBehaviour 
{

    public static UIManager instance;

    private bool hasOpenUI;
    public static bool HasOpenUI
    {
        get { return BountyManager.instance.HasOpenUI() && SceneManager.instance.HasOpenUI(); }
    }

    public Transform otherPlayersUISpawn;
    [SerializeField] private List<InventorySlot> hotbarSlots = new List<InventorySlot>();

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
