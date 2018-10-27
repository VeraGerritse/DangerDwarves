using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class UIManager : MonoBehaviour
{

    public static UIManager instance;

    private bool hasOpenUI;
    public static bool HasOpenUI
    {
        get
        {
            if (BountyManager.instance.HasOpenUI() || SceneManager.instance.HasOpenUI() || ArmoryManager.instance.HasOpenUI())
            {
                return true;
            }
            return false;
        }
    }

    public Transform otherPlayersUISpawn;
    [HideInInspector] public PlayerStatusCanvas playerStatusCanvas;
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

        playerStatusCanvas = GetComponentInChildren<PlayerStatusCanvas>();

        SettingsManager.instance.mpInfoText.text = "Room Name\n<color=red>" + Photon.Pun.PhotonNetwork.CurrentRoom.Name + "</color>\nState\n<color=red>" + Photon.Pun.PhotonNetwork.NetworkClientState;
    }
}
