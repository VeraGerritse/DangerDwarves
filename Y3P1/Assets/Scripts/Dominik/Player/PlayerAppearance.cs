using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerAppearance : MonoBehaviourPunCallbacks
{

    [Header("Appearance")]

    [SerializeField] private List<GameObject> beardObjects = new List<GameObject>();
    [SerializeField] private List<Material> beardMaterials = new List<Material>();
    [SerializeField] private List<Material> bodyMaterials = new List<Material>();
    private SkinnedMeshRenderer dwarfRenderer;

    [Space(10)]

    [SerializeField] private TMP_Dropdown beardObjectDropdown;
    [SerializeField] private TMP_Dropdown beardMatDropdown;
    [SerializeField] private TMP_Dropdown bodyMatDropdown;

    private void Awake()
    {
        dwarfRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        beardObjectDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.AllBuffered, i, beardMatDropdown.value, bodyMatDropdown.value); });
        beardMatDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.AllBuffered, beardObjectDropdown.value, i, bodyMatDropdown.value); });
        bodyMatDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.AllBuffered, beardObjectDropdown.value, beardMatDropdown.value, i); });
    }

    public void RandomizeAppearance()
    {
        int randomBeardModel = Random.Range(0, beardObjects.Count);
        int randomBeardMat = Random.Range(0, beardMaterials.Count);
        int randomDwarfMat = Random.Range(0, bodyMaterials.Count);

        beardObjectDropdown.value = randomBeardModel;
        beardMatDropdown.value = randomBeardMat;
        bodyMatDropdown.value = randomDwarfMat;

        photonView.RPC("SetAppearance", RpcTarget.AllBuffered, randomBeardModel, randomBeardMat, randomDwarfMat);
    }

    [PunRPC]
    private void SetAppearance(int beardModel, int beardMat, int bodyMat)
    {
        for (int i = 0; i < beardObjects.Count; i++)
        {
            if (i == beardModel)
            {
                beardObjects[i].SetActive(true);
                beardObjects[i].GetComponent<Renderer>().material = beardMaterials[beardMat];
            }
            else
            {
                beardObjects[i].SetActive(false);
            }
        }

        if (!dwarfRenderer)
        {
            dwarfRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        }
        dwarfRenderer.material = bodyMaterials[bodyMat];
    }
}
