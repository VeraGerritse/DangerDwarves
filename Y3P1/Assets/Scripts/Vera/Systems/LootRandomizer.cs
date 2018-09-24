using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootRandomizer : MonoBehaviour {
    public List<Weapon_Ranged> test = new List<Weapon_Ranged>();
    public int testItemLevel;
    public static LootRandomizer instance;


    [SerializeField] private int commonPercent;
    [SerializeField] private int rarePercent;
    [SerializeField] private int epicPercent;

    [SerializeField] private int minDamage = 5;

    [SerializeField] private int amountTypes = 1;
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
            DropLoot(1);
        }
    }

    public Item DropLoot(int currentItemLevel)
    {
        int randomType = Random.Range(0, amountTypes);
        Item newItem = null;
        switch (randomType)
        {
            case 0:
                newItem = RangedWeapon(currentItemLevel);
                break;
        }     

        return newItem;
    }

    private Item RangedWeapon(int currentItemLevel)
    {
        Item testItem = new Weapon_Ranged();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);
        int degreesSecun = Degrees();
        int degreesPri = Degrees();
        int amountSecun = 1;
        int amountPrim = 1;
        if(degreesSecun != 0)
        {
            amountSecun = AmountSecun();
        }
        if(degreesPri != 0)
        {
            amountPrim = AmountPrimary();
        }

        bool rOL = false;
        if(rarity >= 2)
        {
            rOL = true;
        }

        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetCrossbowName(), rarity, Database.hostInstance.GetCrossbowSprite(), new Stats(), Database.hostInstance.GetCrossbowObject(),nIL);
        testItem.StartWeapon(BaseDamage(nIL),FireRate(), Database.hostInstance.GetSecundary(rOL), SecundaryFR(), ChargeTime(), Force(), amountSecun, degreesSecun);
        testItem.StartRanged(Force(), amountPrim, degreesPri);
        //end item creation

        test.Add((Weapon_Ranged)testItem);
        return testItem;
    }

    private int AmountPrimary()
    {
        return Random.Range(1, 12);
    }

    private int AmountSecun()
    {
        return Random.Range(1, 6);
    }

    private float Force()
    {
        return Random.Range(15.0f, 20.0f);
    }

    private int Degrees()
    {
        int rand = Random.Range(0, 2);
        if(rand == 0)
        {
            return 0;
        }
        return Random.Range(10, 180);
    }

    private float ChargeTime()
    {
        int rand = Random.Range(0, 3);
        if(rand == 0)
        {
            return 0;
        }
        return Random.Range(0.5f, 1.5f);
    }

    private float SecundaryFR()
    {
        return Random.Range(2.0f, 4.0f);
    }

    private float FireRate()
    {
        return Random.Range(0.2f, 0.5f);
    }

    private int BaseDamage(int IL)
    {
        return Mathf.RoundToInt(minDamage * ((IL / 10) + 1));
    }

    private int NewItemLevel(int rarity, int lastIL)
    {
        rarity -= 1;
        lastIL += rarity;
        if(lastIL == 0)
        {
            lastIL++;
        }
        return lastIL;
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
