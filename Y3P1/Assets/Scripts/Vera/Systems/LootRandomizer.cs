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

    }



    public Item DropLoot(int currentItemLevel, int type)
    {
        int dontDrop = Random.Range(0, 100);
        int goldChance = 50;
        if (type != 3)
        {
            if(type == 1)
            {
                if (dontDrop < 50)
                {
                    return null;
                }
            }
            if(type == 2)
            {
                goldChance = 75;
            }
            int drop = Random.Range(0, 100);
            if (drop < goldChance)
            {
                return LootGold(currentItemLevel);
            }
        }
        int randomType = Random.Range(0, 4);
        Item newItem = null;
        switch (randomType)
        {
            case 0:
                newItem = LootWeapon(currentItemLevel);
                break;         
            case 1:
                newItem = LootHelmet(currentItemLevel);
                break;
            case 2:
                newItem = LootTrinket(currentItemLevel);
                break;
            case 3:
                newItem = LootPotion();
                break;
        }     
        return newItem;
    }

    private Item LootWeapon(int cI)
    {
        int randomType = Random.Range(0, 5);
        Item newItem = null;
        switch (randomType)
        {
            case 0:
                newItem = LootCrossbow(cI);
                break;
            case 1:
                newItem = LootAxe(cI);
                break;
            case 2:
                newItem = LootHammer(cI);
                break;
            case 3:
                newItem = LootSword(cI);
                break;
            case 4:
                newItem = LootOtherWeapon(cI);
                break;
        }

        return newItem;
    }

    private Item LootOtherWeapon(int currentItemLevel)
    {
        Item otherWeapon = new Weapon_Melee();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);
        int myItem = Database.hostInstance.OW();
        bool rOL = false;
        if (rarity >= 2)
        {
            rOL = true;
        }
        string secun = Database.hostInstance.GetMeleeSecundary(rOL);
        otherWeapon.StartUp(Database.hostInstance.GetOWName(myItem), rarity, Database.hostInstance.GetOWSprite(myItem), NewStats(nIL), Database.hostInstance.GetOWObject(myItem), nIL);
        otherWeapon.StartWeapon(BaseDamage(nIL), FireRate(), secun, SecundaryFR(), ChargeTime(), Force(), 1, 0, Buff(secun), Single(secun));
        otherWeapon.StartMelee(Range(), 0);
        return otherWeapon;
    }

    private Item LootGold(int currentItemLevel)
    {
        Item newItem = new Gold();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);
        int amount = GoldAmount(nIL,rarity);
        newItem.StartUp("", 0, 0, null, Database.hostInstance.GetGoldObject(rarity), nIL);
        newItem.StartGold(amount);
        return newItem;
    }

    private Item LootPotion()
    {
        if(Random.Range(0,100) < 50)
        {
            return null;
        }
        Item newPotion = new Potion();
        int rarity = Rarity();
        int typePotion = newPotion.StartPotion(rarity);
        //newPotion.StartUp("", rarity, Database.hostInstance.GetPotionSprite(typePotion) , null, Database.hostInstance.GetPotionObject(typePotion), 1);
        return newPotion;
    }

    private Item LootAxe(int currentItemLevel)
    {
        Item testItem = new Weapon_Melee();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);

        bool rOL = false;
        if (rarity >= 2)
        {
            rOL = true;
        }
        string secun = Database.hostInstance.GetMeleeSecundary(rOL);
        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetAxeName(), rarity, Database.hostInstance.GetAxeSprite(), NewStats(nIL), Database.hostInstance.GetAxeObject(), nIL);
        testItem.StartWeapon(BaseDamage(nIL), FireRate(), secun , SecundaryFR(), ChargeTime(), Force(), 1, 0, Buff(secun),Single(secun));
        testItem.StartMelee(Range(), 0);
        //end item creation
        return testItem;
    }

    private Item LootSword(int currentItemLevel)
    {
        Item testItem = new Weapon_Melee();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);
        int degreesSecun = Degrees();
        int degreesPri = Degrees();
        int amountSecun = 1;
        int amountPrim = 1;
        if (degreesSecun != 0)
        {
            amountSecun = AmountSecun();
        }
        if (degreesPri != 0)
        {
            amountPrim = AmountPrimary();
        }

        bool rOL = false;
        if (rarity >= 2)
        {
            rOL = true;
        }
        string secun = Database.hostInstance.GetMeleeSecundary(rOL);
        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetSwordName(), rarity, Database.hostInstance.GetSwordSprite(), NewStats(nIL), Database.hostInstance.GetSwordObject(), nIL);
        testItem.StartWeapon(BaseDamage(nIL), FireRate(), secun, SecundaryFR(), ChargeTime(), Force(), 1, 0, Buff(secun), Single(secun));
        testItem.StartMelee(Range(), 0);
        //end item creation
        return testItem;
    }

    private Item LootHammer(int currentItemLevel)
    {
        Item testItem = new Weapon_Melee();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);
        int degreesSecun = Degrees();
        int degreesPri = Degrees();
        int amountSecun = 1;
        int amountPrim = 1;
        if (degreesSecun != 0)
        {
            amountSecun = AmountSecun();
        }
        if (degreesPri != 0)
        {
            amountPrim = AmountPrimary();
        }

        bool rOL = false;
        if (rarity >= 2)
        {
            rOL = true;
        }
        string secun = Database.hostInstance.GetMeleeSecundary(rOL);
        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetHammerName(), rarity, Database.hostInstance.GetHammerSprite(), NewStats(nIL), Database.hostInstance.GetHammerObject(), nIL);
        testItem.StartWeapon(BaseDamage(nIL), FireRate(), secun, SecundaryFR(), ChargeTime(), Force(), 1, 0, Buff(secun), Single(secun));
        testItem.StartMelee(Range(), Knockback());
        //end item creation
        return testItem;
    }

    private Item LootCrossbow(int currentItemLevel)
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
        string secun = Database.hostInstance.GetRangedSecundary(rOL);
        
        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetCrossbowName(), rarity, Database.hostInstance.GetCrossbowSprite(), NewStats(nIL), Database.hostInstance.GetCrossbowObject(),nIL);
        testItem.StartWeapon(BaseDamage(nIL),FireRate(), secun, SecundaryFR(), ChargeTime(), Force(), amountSecun, degreesSecun,Buff(secun), Single(secun));
        testItem.StartRanged(Force(), amountPrim, degreesPri);
        //end item creation

        test.Add((Weapon_Ranged)testItem);
        return testItem;
    }

    private int GoldAmount(int currentItemLevel,int rarity)
    {
        float gold = 0;
        if(rarity == 0)
        {
            gold = Random.Range(1, 6);
        }
        else if (rarity == 1)
        {
            gold = Random.Range(6, 16);
        }
        else if (rarity == 2)
        {
            gold = Random.Range(16, 31);
        }
        else if (rarity == 3)
        {
            gold = Random.Range(31, 51);
        }

        int g = Mathf.RoundToInt(gold * (currentItemLevel / 10 + 1));
        return g;
    }

    private Item LootHelmet(int currentItemLevel)
    {
        Item testItem = new Helmet();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);

        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetHelmetName(), rarity, Database.hostInstance.GetHelmetSprite(), NewStats(nIL), Database.hostInstance.GetHelmetObject(), nIL);
        //end item creation
        return testItem;
    }

    private Item LootTrinket(int currentItemLevel)
    {
        Item testItem = new Trinket();
        int rarity = Rarity();
        int nIL = NewItemLevel(rarity, currentItemLevel);

        //Item Creation XD
        testItem.StartUp(Database.hostInstance.GetTrinketName(), rarity, Database.hostInstance.GetTrinketSprite(), NewStats(nIL), Database.hostInstance.GetTrinketObject(), nIL);
        //end item creation
        return testItem;
    }

    private bool Buff(string secunName)
    {
        for (int i = 0; i < Database.hostInstance.secundaryBuffs.Count; i++)
        {
            if (Database.hostInstance.secundaryBuffs[i] == secunName)
            {
                return true;
            }
        }
        return false;
    }

    public Stats NewStats(int itemLvl)
    {
        Stats nS = new Stats();
        float ilvl = itemLvl;
        float times = (ilvl / 10 + 1) - 0.1f;
        nS.strength = Mathf.RoundToInt(Random.Range(1, 5) *times) ;
        nS.stamina = Mathf.RoundToInt(Random.Range(1, 5) * times);
        nS.agility = Mathf.RoundToInt(Random.Range(1, 5) * times);
        nS.willpower = Mathf.RoundToInt(Random.Range(1, 5) * times);
        nS.defense = Mathf.RoundToInt(Random.Range(1, 5) * times);

        return nS;
    }

    private bool Single(string secunName)
    {
        for (int i = 0; i < Database.hostInstance.singleSecondary.Count; i++)
        {
            if (Database.hostInstance.singleSecondary[i] == secunName)
            {
                return true;
            }
        }
        return false;
    }

    private float Range()
    {
        return Random.Range(1.7f, 2.5f);
    }

    private float Knockback()
    {
        return Random.Range(1f, 3.5f);
    }

    private int AmountPrimary()
    {
        return Random.Range(1, 5);
    }

    private int AmountSecun()
    {
        return Random.Range(1, 3);
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
        return Random.Range(10, 90);
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

    private int BaseDamage(float IL)
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
