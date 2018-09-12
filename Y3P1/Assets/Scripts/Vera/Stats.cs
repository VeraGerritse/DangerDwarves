using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Data/Stats")]
public class Stats : ScriptableObject
{

    public int stamina;
    public int strength;
    public int agility;
    public int willpower;
    public int defense;
}
