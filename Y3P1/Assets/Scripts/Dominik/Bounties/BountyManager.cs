using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class BountyManager : MonoBehaviour
{

    public static BountyManager instance;

    [SerializeField] private GameObject bountyCanvas;
    [SerializeField] private GameObject availableBountiesOverview;
    [SerializeField] private GameObject activeBountyOverview;
    [SerializeField] private Transform activeBountySpawn;
    [SerializeField] private GameObject bountyUIPrefab;
    [SerializeField] private Transform availableBountySpawn;

    [Space(10)]

    [SerializeField] private List<Bounty> availableBounties = new List<Bounty>();

    [System.Serializable]
    public class Bounty
    {
        public string bountyName;
        [TextArea] public string bountyDescription;
        public int entityID;
        public int amountToKill;
        public int progress;
        public int goldReward;
        public enum BountyState { InActive, Active, Completed };
        [HideInInspector] public BountyState bountyState;

        public void Reset()
        {
            amountToKill = 0;
            progress = 0;
            bountyState = BountyState.InActive;
        }
    }

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

        ToggleBountyCanvas(false);
        SetupAvailableBountyUI();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (bountyCanvas.activeInHierarchy)
            {
                ToggleBountyCanvas(false);
            }
        }
    }

    public void ToggleBountyCanvas(bool b)
    {
        bountyCanvas.SetActive(b);
    }

    private void SetupAvailableBountyUI()
    {
        for (int i = 0; i < availableBounties.Count; i++)
        {
            BountyUI newBountyUI = Instantiate(bountyUIPrefab, availableBountySpawn.position, Quaternion.identity, availableBountySpawn).GetComponent<BountyUI>();
            newBountyUI.Setup(availableBounties[i]);
        }
    }

    public void ActivateBounty(string bountyName)
    {
        for (int i = 0; i < availableBounties.Count; i++)
        {
            availableBounties[i].Reset();
            availableBounties[i].bountyState = availableBounties[i].bountyName == bountyName ? Bounty.BountyState.Active : Bounty.BountyState.InActive;
        }

        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has started the bounty: <color=yellow>" + bountyName + "</color>!");
    }

    public void RegisterKill(int entityID)
    {
        for (int i = 0; i < availableBounties.Count; i++)
        {
            if (availableBounties[i].bountyState == Bounty.BountyState.Active)
            {
                if (availableBounties[i].entityID == entityID)
                {
                    availableBounties[i].progress++;

                    if (availableBounties[i].progress == availableBounties[i].amountToKill)
                    {
                        CompleteBounty(availableBounties[i].bountyName);
                        return;
                    }
                }
            }
        }
    }

    private void CompleteBounty(string bountyName)
    {
        Bounty completedBounty = null;

        for (int i = 0; i < availableBounties.Count; i++)
        {
            if (availableBounties[i].bountyName == bountyName)
            {
                completedBounty = availableBounties[i];
            }
        }

        if (completedBounty != null)
        {
            completedBounty.bountyState = Bounty.BountyState.Completed;
            Player.localPlayer.myInventory.UpdateGold(completedBounty.goldReward);
        }
    }
}
