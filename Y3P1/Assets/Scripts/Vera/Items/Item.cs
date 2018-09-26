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
    public int itemLevel = 1;    

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
        StatsInfo.instance.SetText(ItemInfo(), WeaponInfo(), RangedInfo(), MeleeInfo(), HelmetInfo(), TrinketInfo());
    }
    
    public string[] ItemInfo()
    {
        string[] newInfo;
        if (myStats != null)
        {
            newInfo = new string[] { itemName, itemRarity.ToString(), "Itemlevel: " + itemLevel.ToString(), "Stamina: " + myStats.stamina.ToString(), "Strength:" + myStats.strength.ToString(), "Agility: " + myStats.agility.ToString(), "WillPower: " + myStats.willpower.ToString(), "Defence: " + myStats.defense.ToString() };
        }
        else
        {
            newInfo = new string[] { itemName, itemRarity.ToString(), "Itemlevel: " + itemLevel.ToString()};
        }
        return newInfo;
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

    public virtual void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS)
    {

    }

    public virtual void StartRanged(float fP, int aP, int dP)
    {

    }

    public virtual void StartMelee(float range,float knockback)
    {

    }

}
