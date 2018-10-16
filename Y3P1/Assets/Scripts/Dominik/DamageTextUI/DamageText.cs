using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{

    private Vector3 newPos;
    private Color newColor;
    private Vector3 originalScale;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float randomSpawnOffsetX = 0.5f;
    [SerializeField] private float randomSpawnOffsetY = 0.1f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Initialise(int damage)
    {
        SetTextColor(damage);

        damageText.text = Mathf.Abs(damage).ToString();
        transform.position += new Vector3(Random.Range(-randomSpawnOffsetX, randomSpawnOffsetX), Random.Range(-randomSpawnOffsetY, randomSpawnOffsetY), 0);

        newPos = transform.position;
        newColor = damageText.color;

        transform.localScale = originalScale;
        transform.localScale *= Remap(Mathf.Abs(damage), 0, 10000, 1, 4f);
    }

    public void Initialise(string status)
    {
        SetTextColor(0);

        damageText.text = status;
        transform.position += new Vector3(Random.Range(-randomSpawnOffsetX, randomSpawnOffsetX), Random.Range(-randomSpawnOffsetY, randomSpawnOffsetY), 0);

        newPos = transform.position;
        newColor = damageText.color;

        transform.localScale = originalScale;
        transform.localScale *= 0.8f;
    }

    private void Update()
    {
        if (damageText.color.a > 0.1f)
        {
            // Im creating the Vector3 newPos only once as a variable and update it in Update.
            // This is faster than using new Vector() since i only create the vector3 once instead of a new one each frame.
            newPos.y += Time.deltaTime * moveSpeed;
            transform.position = newPos;

            // Same for the text color.
            newColor.a -= Time.deltaTime * fadeSpeed;
            damageText.color = newColor;
        }
        else
        {
            ObjectPooler.instance.AddToPool("DamageText", gameObject);
        }
    }

    private void SetTextColor(int damage)
    {
        // Damage.
        if (damage < 0)
        {
            damageText.color = Color.red;
        }
        // Nothing.
        else if (damage == 0)
        {
            damageText.color = Color.white;
        }
        // Heal.
        else if (damage > 0)
        {
            damageText.color = Color.green;
        }
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}
