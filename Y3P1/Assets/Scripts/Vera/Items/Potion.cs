using UnityEngine;
using Y3P1;

[System.Serializable]
public class Potion : Item
{

    private float nextDrinkTime;

    // StatusEffect type and how long the buffs sits on the inflicted target.
    public StatusEffects.StatusEffectType effectType;
    public float statusEffectDuration = 3f;

    // The amount of time this buff lasts on the player who drank it.
    public float buffDuration = 6f;
    public float potionDrinkCooldown = 30f;

    public override void StartUp(string name, int rarity, int Mysprite, Stats myStat, int myObj, int iLevel)
    {
        base.StartUp(name, rarity, Mysprite, myStat, myObj, iLevel);

        itemName = GetPotionName();
        statusEffectDuration = GetStatusEffectDuration();
        buffDuration = GetBuffDuration();
    }

    public void Drink()
    {
        if (Time.time >= nextDrinkTime)
        {
            nextDrinkTime = Time.time + potionDrinkCooldown;
            Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = effectType, statusEffectDuration = statusEffectDuration, endTime = Time.time + buffDuration }, buffDuration);
        }
    }

    private string GetPotionName()
    {
        string s = "(Debug) Potion.GetPotionName() fucked up.";

        switch (effectType)
        {
            case StatusEffects.StatusEffectType.Bleed:

                s = "Potion of Bleeding";
                break;
            case StatusEffects.StatusEffectType.Slow:

                s = "Potion of Slowness";
                break;
            case StatusEffects.StatusEffectType.ArmorBreak:

                s = "Potion of Broken Armor";
                break;
            case StatusEffects.StatusEffectType.WeaponBreak:

                s = "Potion of Broken Weapons";
                break;
            case StatusEffects.StatusEffectType.Poison:

                s = "Potion of Poison";
                break;
        }

        return s;
    }

    private float GetBuffDuration()
    {
        float duration = buffDuration;

        switch (itemRarity)
        {
            case ItemRarity.common:

                duration = 4f;
                break;
            case ItemRarity.rare:

                duration = 5.5f;
                break;
            case ItemRarity.epic:

                duration = 7f;
                break;
            case ItemRarity.legendary:

                duration = 10f;
                break;
        }

        return duration;
    }

    private float GetStatusEffectDuration()
    {
        float duration = statusEffectDuration;

        switch (effectType)
        {
            case StatusEffects.StatusEffectType.Bleed:

                duration = 3f;
                break;
            case StatusEffects.StatusEffectType.Slow:

                duration = 2f;
                break;
            case StatusEffects.StatusEffectType.ArmorBreak:

                duration = 5f;
                break;
            case StatusEffects.StatusEffectType.WeaponBreak:

                duration = 4f;
                break;
            case StatusEffects.StatusEffectType.Poison:

                duration = 3f;
                break;
        }

        return duration;
    }
}
