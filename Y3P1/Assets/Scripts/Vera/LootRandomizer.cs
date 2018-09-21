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
            DropLoot();
        }
    }


    public Item DropLoot()
    {
        Item testItem = new Weapon_Ranged();
        testItem.StartUp(Database.hostInstance.GetCrossbowName(), Rarity(), Database.hostInstance.GetCrossbowSprite(),new Stats(),Database.hostInstance.GetCrossbowObject());

        // TEST SPECIAL RANDOMIZER.
        // Wilde ff andere specials testen.
        string[] secondaries = new string[]
            {
                "Arrow_Homing",
                "Arrow_Volley",
                "Arrow_Explosive",
                "Arrow_Animated"
            };
        (testItem as Weapon).secondaryProjectile = secondaries[Random.Range(0, secondaries.Length)];
        //////////////////////////////////////

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
