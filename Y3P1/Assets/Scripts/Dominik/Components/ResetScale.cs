using UnityEngine;

public class ResetScale : MonoBehaviour 
{

    [SerializeField] private Vector3 scale;

    private void OnDisable()
    {
        transform.localScale = scale;
    }
}
