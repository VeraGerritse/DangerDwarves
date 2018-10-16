using UnityEngine;

[System.Serializable]
public class Stats
{

    private float defenseEffectiveness = 1;
    public float DefenseEffectiveness
    {
        get { return defenseEffectiveness; }
        set { defenseEffectiveness = Mathf.Clamp01(value); }
    }

    private float damageEffectiveness = 1;
    public float DamageEffectiveness
    {
        get { return damageEffectiveness; }
        set { damageEffectiveness = Mathf.Clamp01(value); }
    }

    public int stamina;
    public int strength;
    public int agility;
    public int willpower;
    public int defense;
}
