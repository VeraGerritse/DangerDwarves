using UnityEngine;

public class Randomizer : MonoBehaviour 
{

    private void Awake()
    {
        int i = Random.Range(0, 2);
        gameObject.SetActive(i == 0 ? true : false);

        if (gameObject.activeInHierarchy)
        {
            int ii = Random.Range(0, 2);
            transform.localScale = new Vector3(ii == 0 ? 1 : -1, transform.localScale.y, transform.localScale.z);
        }
    }
}
