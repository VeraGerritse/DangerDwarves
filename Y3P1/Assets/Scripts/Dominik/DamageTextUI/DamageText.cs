using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour 
{

    private Vector3 newPos;
    private Color newColor;

    [SerializeField] private TextMeshProUGUI damageText;
    [SerializeField] private float randomSpawnOffset = 0.5f;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;

    public void Initialise(int damage)
    {
        SetTextColor(damage);

        damageText.text = damage.ToString();
        transform.position += new Vector3(Random.Range(-randomSpawnOffset, randomSpawnOffset), 0, 0);

        newPos = transform.position;
        newColor = damageText.color;
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
            // TODO: Add to object pooler.
            Destroy(gameObject);
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
}
