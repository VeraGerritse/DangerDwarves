using UnityEngine;

public class UIManager : MonoBehaviour 
{

    public static UIManager instance;

    public static bool hasOpenUI;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }
}
