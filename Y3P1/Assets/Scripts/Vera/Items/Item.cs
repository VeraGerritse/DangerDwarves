
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
    

    //public string Information()
    //{
    //    string allInfo = itemName + "\n" 
    //}

    //public string rarityText()
    //{

    //}

    public virtual void StartWeapon(int baseDamage_, float fireRate, string sS, float sFR, float charge, float fS, int aS, int dS)
    {

    }

    public virtual void StartRanged(float fP, int aP, int dP)
    {

    }

}
