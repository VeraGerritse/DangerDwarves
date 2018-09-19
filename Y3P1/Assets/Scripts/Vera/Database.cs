using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {

    public static Database hostInstance;
    [Header("Weapons")]
    [Header("Crossbow")]
    [SerializeField] private List<string> crossbowNames = new List<string>();
    [SerializeField] private List<Sprite> crossbowSprite = new List<Sprite>();
    [SerializeField] private List<GameObject> crossbowObject = new List<GameObject>();
    [Header("Axe")]
    [SerializeField] private List<string> axeNames = new List<string>();
    [SerializeField] private List<Sprite> axeSprite = new List<Sprite>();
    [SerializeField] private List<GameObject> axeObject = new List<GameObject>();
    [Header("sword")]
    [SerializeField] private List<string> swordNames = new List<string>();
    [SerializeField] private List<Sprite> swordSprite = new List<Sprite>();
    [SerializeField] private List<GameObject> swordObject = new List<GameObject>();
    [Header("Helmet")]
    [SerializeField] private List<string> HelmetNames = new List<string>();
    [SerializeField] private List<Sprite> HelmetSprite = new List<Sprite>();
    [SerializeField] private List<GameObject> HelmetObject = new List<GameObject>();
    [Header("Trinket")]
    [SerializeField] private List<string> trinketNames = new List<string>();
    [SerializeField] private List<Sprite> trinketSprite = new List<Sprite>();




    public void Awake()
    {
        if(hostInstance == null)
        {
            hostInstance = this;
        }
    }

    public string GetHelmetName()
    {
        return HelmetNames[Random.Range(0, HelmetNames.Count)];
    }

    public Sprite GetSpriteCrossbow()
    {
        return crossbowSprite[Random.Range(0, crossbowSprite.Count)];
    }
    public string GetWeaponName()
    {
        return "weapon";
    }
}
