using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class BountyManager : MonoBehaviourPunCallbacks
{

    public static BountyManager instance;
    public static Bounty activeBounty;

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
            progress = 0;
            bountyState = BountyState.InActive;
        }

        public void Complete()
        {
            bountyState = BountyState.Completed;
            Y3P1.Player.localPlayer.myInventory.UpdateGold(goldReward);
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
        photonView.RPC("SyncActiveBounty", RpcTarget.All, bountyName, 0);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>started</b> the bounty: <color=yellow>" + bountyName + "</color>!");
    }

    [PunRPC]
    private void SyncActiveBounty(string bountyName, int progress)
    {
        if (activeBounty != null)
        {
            return;
        }

        for (int i = 0; i < availableBounties.Count; i++)
        {
            availableBounties[i].Reset();

            if (availableBounties[i].bountyName == bountyName)
            {
                activeBounty = availableBounties[i];
                activeBounty.bountyState = Bounty.BountyState.Active;
                activeBounty.progress = progress;

                BountyUI newBountyUI = Instantiate(bountyUIPrefab, activeBountySpawn.position, Quaternion.identity, activeBountySpawn).GetComponent<BountyUI>();
                newBountyUI.Setup(activeBounty);
            }
        }

        ToggleView();
    }

    public void CancelBounty(string bountyName)
    {
        photonView.RPC("SyncCancelBounty", RpcTarget.All);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>canceled</b> the bounty: <color=yellow>" + bountyName + "</color>!");
    }

    [PunRPC]
    private void SyncCancelBounty()
    {
        activeBounty.Reset();
        activeBounty = null;
        Destroy(activeBountySpawn.childCount > 0 ? activeBountySpawn.GetChild(0).gameObject : null);
        ToggleView();
    }

    public void CompleteBounty(string bountyName)
    {
        photonView.RPC("SyncCompleteBounty", RpcTarget.All);
        NotificationManager.instance.NewNotification("<color=yellow>" + PhotonNetwork.NickName + "</color> has <b>completed</b> the bounty: <color=yellow>" + bountyName + "</color>!");
    }

    [PunRPC]
    private void SyncCompleteBounty()
    {
        activeBounty.Complete();
        activeBounty = null;
        Destroy(activeBountySpawn.childCount > 0 ? activeBountySpawn.GetChild(0).gameObject : null);
        ToggleView();
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
                }
            }
        }
    }

    private void ToggleView()
    {
        availableBountiesOverview.SetActive(activeBounty == null ? true : false);
        activeBountyOverview.SetActive(activeBounty == null ? false : true);
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        if (activeBounty != null)
        {
            photonView.RPC("SyncActiveBounty", RpcTarget.All, activeBounty.bountyName, activeBounty.progress);
        }
    }
}
