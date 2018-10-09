using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BountyUI : MonoBehaviour
{

    private BountyManager.Bounty myBounty;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI bountyNameText;
    [SerializeField] private TextMeshProUGUI bountyDescText;
    [SerializeField] private TextMeshProUGUI bountyAmountText;
    [SerializeField] private TextMeshProUGUI bountyGoldRewardText;

    [Header("Progress")]
    [SerializeField] private TextMeshProUGUI bountyProgressText;
    [SerializeField] private GameObject bountyProgressPanel;
    [SerializeField] private Image bountyProgressFill;

    [Header("Buttons")]
    [SerializeField] private Button startBountyButton;
    [SerializeField] private Button cancelBountyButton;
    [SerializeField] private Button completeBountyButton;

    public void Setup(BountyManager.Bounty bounty)
    {
        myBounty = bounty;

        bountyNameText.text = bounty.bountyName;
        bountyDescText.text = bounty.bountyDescription;
        bountyAmountText.text = "Amount to kill: <color=red>" + bounty.amountToKill.ToString();
        bountyGoldRewardText.text = "Reward: <color=yellow>" + bounty.goldReward.ToString() + "</color> gold";
        bountyProgressPanel.SetActive(false);

        SetupButtonListeners();
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && BountyManager.activeBounty == myBounty)
        {
            bountyProgressFill.fillAmount = (float)BountyManager.activeBounty.progress / BountyManager.activeBounty.amountToKill;
            bountyProgressText.text = "<color=red>" + BountyManager.activeBounty.progress.ToString() + "</color> / <color=red>" + BountyManager.activeBounty.amountToKill.ToString();

            if (!bountyProgressPanel.activeInHierarchy)
            {
                bountyProgressPanel.SetActive(true);
                SetButtonState(BountyManager.Bounty.BountyState.Active);
            }

            if (BountyManager.activeBounty.progress >= BountyManager.activeBounty.amountToKill)
            {
                SetButtonState(BountyManager.Bounty.BountyState.Completed);
            }
        }
    }

    private void SetupButtonListeners()
    {
        startBountyButton.onClick.AddListener(() => BountyManager.instance.ActivateBounty(myBounty.bountyName));
        cancelBountyButton.onClick.AddListener(() => BountyManager.instance.CancelBounty(myBounty.bountyName));
        completeBountyButton.onClick.AddListener(() => BountyManager.instance.CompleteBounty(myBounty.bountyName));
    }

    private void SetButtonState(BountyManager.Bounty.BountyState state)
    {
        switch (state)
        {
            case BountyManager.Bounty.BountyState.InActive:

                startBountyButton.transform.parent.gameObject.SetActive(true);
                cancelBountyButton.transform.parent.gameObject.SetActive(false);
                completeBountyButton.transform.parent.gameObject.SetActive(false);
                break;
            case BountyManager.Bounty.BountyState.Active:

                startBountyButton.transform.parent.gameObject.SetActive(false);
                cancelBountyButton.transform.parent.gameObject.SetActive(true);
                completeBountyButton.transform.parent.gameObject.SetActive(false);
                break;
            case BountyManager.Bounty.BountyState.Completed:

                startBountyButton.transform.parent.gameObject.SetActive(false);
                cancelBountyButton.transform.parent.gameObject.SetActive(false);
                completeBountyButton.transform.parent.gameObject.SetActive(true);
                break;
        }
    }
}
