using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    private bool initialised;

    [SerializeField] private Image foregroundHealthBar;
    [SerializeField] private Image backgroundHealthBar;
    [SerializeField] private float backgroundLerpTime = 1;

    private void Awake()
    {
        if (!initialised)
        {
            Initialise(GetComponentInParent<Health>());
        }
    }

    public void Initialise(Health health)
    {
        if (health)
        {
            health.OnHealthModified += Health_OnHealthModified;
            initialised = true;
        }
    }

    private void Health_OnHealthModified(float percentage)
    {
        foregroundHealthBar.fillAmount = percentage;
        StartCoroutine(LerpBackgroundHealthBar(percentage));
    }

    private IEnumerator LerpBackgroundHealthBar(float percentage)
    {
        float startPercentage = backgroundHealthBar.fillAmount;
        float progress = 0f;

        while (progress < backgroundLerpTime)
        {
            progress += Time.deltaTime;
            backgroundHealthBar.fillAmount = Mathf.Lerp(startPercentage, percentage, progress / backgroundLerpTime);
            yield return null;
        }

        backgroundHealthBar.fillAmount = percentage;
    }
}
