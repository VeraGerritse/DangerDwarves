using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {

    public static Database hostInstance;
    [SerializeField] private List<string> adjetives = new List<string>();
    [SerializeField] private List<string> armorNouns = new List<string>();
    [SerializeField] private List<string> weaponNouns = new List<string>();

    [SerializeField] private List<Sprite> crossbows = new List<Sprite>();

    public void Awake()
    {
        if(hostInstance == null)
        {
            hostInstance = this;
        }
    }

    public string GetArmorName()
    {
        return armorNouns[Random.Range(0,armorNouns.Count)];
    }

    public Sprite GetSpriteCrossbow()
    {
        return crossbows[Random.Range(0, crossbows.Count)];
    }
    public string GetWeaponName()
    {
        return "weapon";
    }
}
