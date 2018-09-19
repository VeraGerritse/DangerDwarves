using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour 
{

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private TextMeshProUGUI notificationText;

    private void OnEnable()
    {
        notificationText.color = new Color(notificationText.color.r, notificationText.color.g, notificationText.color.b, 1);
    }

    public void Initialise(string text)
    {
        notificationText.text = text;
    }

    private void Update()
    {
        transform.Translate(Vector3.up * moveSpeed);

        if (notificationText.color.a < 0.05f)
        {
            ObjectPooler.instance.AddToPool("Notification", gameObject);
        }
    }
}
