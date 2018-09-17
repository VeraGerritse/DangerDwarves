using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Notification : MonoBehaviour 
{

    private Color newColor;

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float fadeSpeed = 2f;
    [SerializeField] private Image notificationBackground;
    [SerializeField] private TextMeshProUGUI notificationText;

    private void OnEnable()
    {
        notificationText.color = Color.white;
        newColor = notificationText.color;
    }

    public void Initialise(string text)
    {
        notificationText.text = text;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed);

        if (notificationText.color.a > 0.1f)
        {
            newColor.a -= Time.deltaTime * fadeSpeed;
            notificationText.color = newColor;
            notificationBackground.color = newColor;
        }
        else
        {
            ObjectPooler.instance.AddToPool("Notification", gameObject);
        }
    }
}
