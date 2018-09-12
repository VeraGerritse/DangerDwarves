using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{

    private bool initialised;

    private Animator anim;

    [SerializeField] private Image foregroundHealthBar;
    [SerializeField] private Image backgroundHealthBar;
    [SerializeField] private float backgroundLerpTime = 1;

    // TODO: Add this to object pooler and remove variable.
    [SerializeField] private GameObject damageTextPrefab;

    private void Awake()
    {
        anim = GetComponent<Animator>();

        if (!initialised)
        {
            Initialise(transform.root.GetComponentInChildren<Health>());
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

    private void Health_OnHealthModified(float percentage, int? amount)
    {
        // Check if health got decreased or added and play the according animation.
        // TODO: Add 'IncreaseHealth' animation, it just plays the decrease animation now.
        anim.SetTrigger(foregroundHealthBar.fillAmount > percentage ? "DecreaseHealth" : "DecreaseHealth");

        foregroundHealthBar.fillAmount = percentage;
        StartCoroutine(LerpBackgroundHealthBar(percentage));

        if (amount != null)
        {
            DamageText newDamageText = Instantiate(damageTextPrefab, transform.position, transform.rotation).GetComponent<DamageText>();
            newDamageText.Initialise((int)amount);
        }
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
