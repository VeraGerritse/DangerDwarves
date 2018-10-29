using UnityEngine;
[System.Serializable]
public class Item
{

    public string itemName;
    public enum ItemRarity { common = 0, rare = 1, epic = 2, legendary = 3 }
    public ItemRarity itemRarity;
    //public Sprite itemImage;
    public int spriteIndex;
    public Stats myStats;
    // public GameObject itemPrefab;
    public int prefabIndex;
    public int itemLevel = -1;

    public virtual void StartUp(string name, int rarity,int Mysprite,Stats myStat,int myObj,int iLevel)
    {
        itemName = name;
        itemRarity = (ItemRarity)rarity;
        spriteIndex = Mysprite;
        myStats = myStat;
        prefabIndex = myObj;
        itemLevel = iLevel;
    }

    public void SendInfo()
    {
        StatsInfo.instance.SetText(ItemInfo(),DamageInfo() ,WeaponInfo(), RangedInfo(), MeleeInfo(), HelmetInfo(), TrinketInfo());
    }
    
    public string[] ItemInfo()
    {
        string[] newInfo;
        if (myStats != null)
        {
            newInfo = new string[] { RarityInfo(), "Item level: <color=#00A8FF>" + itemLevel.ToString(), "Stamina: <color=#00A8FF>" + myStats.stamina.ToString(), "Strength: <color=#00A8FF>" + myStats.strength.ToString(), "Agility: <color=#00A8FF>" + myStats.agility.ToString(), "WillPower: <color=#00A8FF>" + myStats.willpower.ToString(), "Defence: <color=#00A8FF>" + myStats.defense.ToString() };
        }
        else if (this is Potion)
        {
            newInfo = new string[] {"<color=white>" + itemName, itemRarity.ToString(), Database.hostInstance.potionDiscription[PotionNum()] };
        }
        else
        {
            NotificationManager.instance.NewNotification(itemName);
            newInfo = new string[] { RarityInfo(), "Item level: <color=#00A8FF>" + itemLevel.ToString() };
        }
        
        return newInfo;
    }

    public virtual int PotionNum()
    {
        return -1;
    }

    public virtual int StartPotion(int rarity)
    {
        NotificationManager.instance.NewNotification("AAAAAAAAAAAAAAAAAAAAAAAAAAh");
        return -1;
    }
    public virtual void StartGold(int amount)
    {
        
    }
    public virtual string[] DamageInfo()
    {
        return null;
    }
    public virtual string[] WeaponInfo()
    {
        return null;
    }

    public virtual string[] HelmetInfo()
    {
        return null;
    }

    public virtual string[] TrinketInfo()
    {
        return null;
    }

    public virtual string[] RangedInfo()
    {
        return null;
    }

    public virtual string[] MeleeInfo()
    {
        return null;
    }

    private string RarityInfo()
    {
        if(itemRarity == ItemRarity.common)
        {
            return "<color=white>" + itemName;
        }
        if(itemRarity == ItemRarity.epic)
        {
            return "<color=purple>" + itemName;
        }
        if(itemRarity == ItemRarity.rare)
        {
            return "<color=#00A8FF>" + itemName;
        }
        return "<color=#FFA500>" + itemName;
    }

    public virtual void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS,bool buff,bool single)
    {

    }

    public virtual void StartRanged(float fP, int aP, int dP)
    {

    }

    public virtual void StartMelee(float range,float knockback)
    {

    }

    public virtual void Awake()
    {

    }

}
