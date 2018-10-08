using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecoyPlayer : MonoBehaviour {

    [SerializeField] private List<GameObject> beards = new List<GameObject>();
    [SerializeField] private List<Material> beardMaterials = new List<Material>();
    [SerializeField] private List<Material> skinMaterials = new List<Material>();

    public GameObject currentBeard;

    public Renderer skins;

    public Camera faceCam;
    public Camera bodyCam;

    public void ChangeAppearance(int beard, int beardColor, int skin)
    {
        DisableBeard();
        currentBeard = beards[beard];
        currentBeard.SetActive(true);
        currentBeard.GetComponent<Renderer>().material = beardMaterials[beardColor];
        skins.material = skinMaterials[skin];
    }

    private void DisableBeard()
    {
        currentBeard.SetActive(false);
    }
}
