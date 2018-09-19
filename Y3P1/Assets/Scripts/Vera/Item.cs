
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


    public virtual void StartUp(string name, int rarity,int Mysprite,Stats myStat,int myObj)
    {
        itemName = name;
        itemRarity = (ItemRarity)rarity;
        spriteIndex = Mysprite;
        myStats = myStat;
        prefabIndex = myObj;
    }

}
