using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRandomizer : MonoBehaviour {
    public List<Item> test = new List<Item>();
    public int testItemLevel;
    public static LootRandomizer instance;


    [SerializeField] private int commonPercent;
    [SerializeField] private int rarePercent;
    [SerializeField] private int epicPercent;
    //test shiz

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            for (int i = 0; i < 1000; i++)
            {
                DropLoot();
            }
        }
    }


    public Item DropLoot()
    {
        Item testItem = ScriptableObject.CreateInstance<Weapon_Ranged>();
        testItem.StartUp(Database.hostInstance.GetHelmetName(), Rarity(), Database.hostInstance.GetSpriteCrossbow());
        test.Add(testItem);
        return testItem;
    }

    private int Rarity()
    {
        int chance = 0;
        chance = Random.Range(0, 100);

        if (chance < commonPercent)
        {
            return 0;
        }
        if (chance < rarePercent)
        {
            return 1;
        }
        if (chance < epicPercent)
        {
            return 2;
        }


        return 3;
    }
}
