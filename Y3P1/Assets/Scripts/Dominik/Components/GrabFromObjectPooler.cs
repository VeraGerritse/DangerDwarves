using UnityEngine;

public class GrabFromObjectPooler : MonoBehaviour
{

    public void GrabFromPool(string poolName)
    {
        ObjectPooler.instance.GrabFromPool(poolName, transform.position, Quaternion.identity);
    }
}
