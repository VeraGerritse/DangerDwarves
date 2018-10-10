using UnityEngine;
using TMPro;
using Y3P1;

public class PlayerUI : MonoBehaviour 
{

    private bool isInitialised;
    private Player target;

    [SerializeField] private TextMeshProUGUI nameText;

    private void Update()
    {
        if (!target && isInitialised)
        {
            Destroy(gameObject);
        }
    }

    public void Initialise(Player target, bool local)
    {
        if (!target)
        {
            return;
        }

        this.target = target;
        if (nameText)
        {
            nameText.text = local ? "" : target.photonView.Owner.NickName;
        }

        isInitialised = true;
    }
}
