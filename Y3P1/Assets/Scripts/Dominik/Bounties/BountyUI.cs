using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BountyUI : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI bountyNameText;
    [SerializeField] private TextMeshProUGUI bountyDescText;
    [SerializeField] private TextMeshProUGUI bountyAmountText;
    [SerializeField] private TextMeshProUGUI bountyGoldRewardText;
    [SerializeField] private Button startBountyButton;

    public void Setup(BountyManager.Bounty bounty)
    {
        bountyNameText.text = bounty.bountyName;
        bountyDescText.text = bounty.bountyDescription;
        bountyAmountText.text = "Amount to kill: <color=red>" + bounty.amountToKill.ToString();
        bountyGoldRewardText.text = "Reward: <color=yellow>" + bounty.goldReward.ToString() + "</color> gold";

        startBountyButton.onClick.AddListener(() => BountyManager.instance.ActivateBounty(bounty.bountyName));
    }
}
