using UnityEngine;
using TMPro;

public class Notification : MonoBehaviour 
{

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private TextMeshProUGUI notificationText;

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
