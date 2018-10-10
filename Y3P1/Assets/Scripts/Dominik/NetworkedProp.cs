using UnityEngine;
using Photon.Pun;

public class NetworkedProp : MonoBehaviour 
{

    [SerializeField] private GameObject prop;
    [SerializeField] private float gizmoRadius = 1;
    [SerializeField] private Vector3 gizmoOffset;

    private void Awake()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject propSpawn = PhotonNetwork.InstantiateSceneObject(prop.name, transform.position, transform.rotation);
            propSpawn.transform.SetParent(transform);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + gizmoOffset, gizmoRadius);
    }
}
