using UnityEngine;
using Y3P1;

[System.Serializable]
public class Potion : Item
{

    private float nextDrinkTime;

    // StatusEffect type and how long the buffs sits on the inflicted target.
    public StatusEffects.StatusEffectType effectType;
    public float statusEffectDuration;

    // The amount of time this buff lasts on the player who drank it.
    public float buffDuration = 6f;
    public float potionDrinkCooldown = 30f;

    public void Drink()
    {
        if (Time.time >= nextDrinkTime)
        {
            nextDrinkTime = Time.time + potionDrinkCooldown;
            Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = effectType, statusEffectDuration = statusEffectDuration, endTime = Time.time + buffDuration }, buffDuration);
        }
    }

    // Some itemNames, dont know if this is useful. Feel free to remove.
    public string GetPotionName()
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
}
