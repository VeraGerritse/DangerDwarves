using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class PlayerAppearance : MonoBehaviourPunCallbacks
{

    [Header("Appearance")]

    [SerializeField] private List<GameObject> beardObjects = new List<GameObject>();
    [SerializeField] private List<Material> beardMaterials = new List<Material>();
    [SerializeField] private List<Material> bodyMaterials = new List<Material>();
    public SkinnedMeshRenderer dwarfRenderer;
    public Material beardMat;
    [SerializeField] private DecoyPlayer decoy;
    [Space(10)]

    [SerializeField] private TMP_Dropdown beardObjectDropdown;
    [SerializeField] private TMP_Dropdown beardMatDropdown;
    [SerializeField] private TMP_Dropdown bodyMatDropdown;



    private void Awake()
    {
        dwarfRenderer = GetComponentInChildren<SkinnedMeshRenderer>();

        beardObjectDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.All, i, beardMatDropdown.value, bodyMatDropdown.value); SetDecoyAppearance(beardObjectDropdown.value, beardMatDropdown.value, bodyMatDropdown.value); });
        beardMatDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.All, beardObjectDropdown.value, i, bodyMatDropdown.value); SetDecoyAppearance(beardObjectDropdown.value, beardMatDropdown.value, bodyMatDropdown.value); });
        bodyMatDropdown.onValueChanged.AddListener((int i) => { photonView.RPC("SetAppearance", RpcTarget.All, beardObjectDropdown.value, beardMatDropdown.value, i); SetDecoyAppearance(beardObjectDropdown.value, beardMatDropdown.value, bodyMatDropdown.value); });
    }

    public void RandomizeAppearance()
    {
        int randomBeardModel = Random.Range(0, beardObjects.Count);
        int randomBeardMat = Random.Range(0, beardMaterials.Count);
        int randomDwarfMat = Random.Range(0, bodyMaterials.Count);

        beardObjectDropdown.value = randomBeardModel;
        beardMatDropdown.value = randomBeardMat;
        bodyMatDropdown.value = randomDwarfMat;
        beardMat = beardMaterials[randomBeardMat];
        photonView.RPC("SetAppearance", RpcTarget.All, randomBeardModel, randomBeardMat, randomDwarfMat);
        SetDecoyAppearance(beardObjectDropdown.value, beardMatDropdown.value, bodyMatDropdown.value);
    }

    public void SetDecoyAppearance(int beard, int color, int skin)
    {
        decoy.ChangeAppearance(beard, color, skin);
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

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("SetAppearance", RpcTarget.AllBuffered, beardObjectDropdown.value, beardMatDropdown.value, bodyMatDropdown.value);
    }
}
