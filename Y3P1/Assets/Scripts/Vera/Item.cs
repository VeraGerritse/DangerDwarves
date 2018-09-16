using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom Objects/Items")]
public class Item : ScriptableObject
{
    [Header("Base Item Stats")]
    [SerializeField] private string itemName;
    [SerializeField] private enum ItemRarity {common, rare, epic,legendary}
    [SerializeField] private ItemRarity itemRarity;
    [SerializeField] private Sprite itemImage;
    public Stats myStats;
    public GameObject itemPrefab;
}
