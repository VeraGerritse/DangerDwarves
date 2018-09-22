using UnityEngine;
using UnityEngine.UI;

public class WeaponChargeCanvas : MonoBehaviour
{

    private GameObject chargePanel;
    private float currentChargeTime;
    private float chargeTime;

    [SerializeField] private Image progressImage;

    public void Initialise()
    {
        chargePanel = transform.GetChild(0).gameObject;

        WeaponSlot.OnStartChargeSecondary += WeaponSlot_OnStartChargeSecondary;
        WeaponSlot.OnStopChargeSecondary += WeaponSlot_OnStopChargeSecondary;
    }

    private void Update()
    {
        if (chargePanel.activeInHierarchy)
        {
            currentChargeTime += Time.deltaTime;
            progressImage.fillAmount = currentChargeTime / chargeTime;
        }
    }

    private void WeaponSlot_OnStartChargeSecondary(float chargeTime)
    {
        this.chargeTime = chargeTime;
        currentChargeTime = 0;
        chargePanel.SetActive(true);
    }

    private void WeaponSlot_OnStopChargeSecondary()
    {
        chargePanel.SetActive(false);
    }

    private void OnDisable()
    {
        WeaponSlot.OnStartChargeSecondary -= WeaponSlot_OnStartChargeSecondary;
        WeaponSlot.OnStopChargeSecondary -= WeaponSlot_OnStopChargeSecondary;
    }
}
