using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Database : MonoBehaviour {

    public static Database hostInstance;
    public List<Sprite> allSprites = new List<Sprite>();
    public List<GameObject> allGameobjects = new List<GameObject>();

    [Header("Weapons")]

    [Header("Attacks")]
    [SerializeField] private List<string> secundaryAttacks = new List<string>();
    [SerializeField] private List<string> primaryAttacks = new List<string>();

    [Header("Crossbow")]
    [SerializeField] private List<string> crossbowNames = new List<string>();
    public List<Sprite> crossbowSprite = new List<Sprite>();
    public List<GameObject> crossbowObject = new List<GameObject>();

    [Header("Axe")]
    [SerializeField] private List<string> axeNames = new List<string>();
    public List<Sprite> axeSprite = new List<Sprite>();
    public List<GameObject> axeObject = new List<GameObject>();

    [Header("sword")]
    [SerializeField] private List<string> swordNames = new List<string>();
    public List<Sprite> swordSprite = new List<Sprite>();
    public List<GameObject> swordObject = new List<GameObject>();

    [Header("Helmet")]
    [SerializeField] private List<string> HelmetNames = new List<string>();
    public List<Sprite> HelmetSprite = new List<Sprite>();
    public List<GameObject> HelmetObject = new List<GameObject>();

    [Header("Trinket")]
    [SerializeField] private List<string> trinketNames = new List<string>();
    public List<Sprite> trinketSprite = new List<Sprite>();




    public void Awake()
    {
        if(hostInstance == null)
        {
            hostInstance = this;
        }
        allSprites.Add(null);
        allGameobjects.Add(null);
        allSprites.AddRange(crossbowSprite);
        allGameobjects.AddRange(crossbowObject);

        // Heb ff een axe hieraan toegevoegd om melee weapons te testen.
        allGameobjects.AddRange(axeObject);
    }

    public string GetSecundary(bool legen)
    {
        int rand = Random.Range(0, secundaryAttacks.Count);
        if(legen && rand == 0)
        {
            rand = Random.Range(1, secundaryAttacks.Count);
        }
        return secundaryAttacks[rand];
    }

    public string GetCrossbowName()
    {
        return crossbowNames[Random.Range(0, crossbowNames.Count)];
    }

    public int GetCrossbowSprite()
    {
        int randomSpri = Random.Range(0, crossbowSprite.Count);
        print(randomSpri);
        Sprite mySpri = crossbowSprite[randomSpri] ;
        print(mySpri);
        int index = 0;
        for (int i = 0; i < allSprites.Count; i++)
        {
            if(mySpri == allSprites[i])
            {
                index = i;
            }
        }
        return index;
    }

    public int GetCrossbowObject()
    {
        GameObject myObj = crossbowObject[Random.Range(0, crossbowObject.Count)];
        int index = 0;
        for (int i = 0; i < allGameobjects.Count; i++)
        {
            if(myObj == allGameobjects[i])
            {
                index = i;
            }
        }
        return index;
    }


    //public string GetHelmetName()
    //{
    //    return HelmetNames[Random.Range(0, HelmetNames.Count)];
    //}

    //public Sprite GetSpriteCrossbow()
    //{
    //    return crossbowSprite[Random.Range(0, crossbowSprite.Count)];
    //}
    //public string GetWeaponName()
    //{
    //    return "weapon";
    //}
}
