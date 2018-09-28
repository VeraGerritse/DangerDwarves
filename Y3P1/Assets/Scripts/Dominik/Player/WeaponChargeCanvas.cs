using UnityEngine;
using UnityEngine.UI;
using Y3P1;

public class WeaponChargeCanvas : MonoBehaviour
{

    private GameObject chargePanel;
    private float currentChargeTime;
    private float chargeTime;

    [SerializeField] private Image progressImage;
    [SerializeField] private Animator progressImageAnim;

    private void Awake()
    {
        Player.OnLocalPlayerInitialise += Initialise;
    }

    private void Initialise()
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

            if (progressImage.fillAmount > 0.99f)
            {
                progressImageAnim.SetTrigger("FullCharge");
            }
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
        Player.OnLocalPlayerInitialise -= Initialise;

        WeaponSlot.OnStartChargeSecondary -= WeaponSlot_OnStartChargeSecondary;
        WeaponSlot.OnStopChargeSecondary -= WeaponSlot_OnStopChargeSecondary;
    }
}
