using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Objects/Items")]
public class Item : ScriptableObject
{

    public string itemName;
    public enum ItemRarity { common = 0, rare = 1, epic = 2, legendary = 3 }
    public ItemRarity itemRarity;
    public Sprite itemImage;
    public Stats myStats;
    public GameObject itemPrefab;


    public void StartUp(string name, int rarity, Sprite myItemIma)
    {
        itemName = name;
        this.name = name;
        itemImage = myItemIma;
        itemRarity = (ItemRarity)rarity;
        
        Debug.Log(itemRarity);
    }

}
