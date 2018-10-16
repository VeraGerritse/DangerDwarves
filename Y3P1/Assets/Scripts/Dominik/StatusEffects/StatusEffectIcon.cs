using UnityEngine;
using UnityEngine.UI;

public class StatusEffectIcon : MonoBehaviour
{

    private float duration;

    public StatusEffects.StatusEffectType type;

    [SerializeField] private Image durationFill;

    public void Activate(float? duration = null)
    {
        gameObject.SetActive(true);

        if (durationFill)
        {
            durationFill.fillAmount = 1f;
            this.duration = duration != null ? (float)duration : 0;
        }
    }

    private void Update()
    {
        if (gameObject.activeInHierarchy && durationFill)
        {
            durationFill.fillAmount -= 1f / duration * Time.deltaTime;
        }
    }
}
