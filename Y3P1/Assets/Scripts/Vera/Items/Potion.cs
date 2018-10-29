using UnityEngine;
using Y3P1;

[System.Serializable]
public class Potion : Item
{

    private float nextDrinkTime;
    private int myMaterialIndex;
    private int index;

    // StatusEffect type and how long the buffs sits on the inflicted target.
    public StatusEffects.StatusEffectType effectType;
    public float statusEffectDuration = 3f;

    // The amount of time this buff lasts on the player who drank it.
    public float buffDuration = 6f;
    public float potionDrinkCooldown = 30f;

    public override void Awake()
    {
        nextDrinkTime = 0;
    }

    public override int PotionNum()
    {
        return index;
    }

    public override int StartPotion(int rarity)
    {
        nextDrinkTime = 0;
        index = Random.Range(0, 5);
        Database.hostInstance.GetPotionSprite(index);
        Database.hostInstance.GetPotionObject(index);
        effectType = (StatusEffects.StatusEffectType)index;

        myMaterialIndex = Database.hostInstance.GetPotionMaterial(index);
        spriteIndex = Database.hostInstance.GetPotionSprite(index);
        prefabIndex = Database.hostInstance.GetPotionObject(index);

        itemName = GetPotionName();
        Debug.Log(itemName);
        itemRarity = (ItemRarity)rarity;
        statusEffectDuration = GetStatusEffectDuration();
        buffDuration = GetBuffDuration();
        itemLevel = 1;

        return index;
    }

    public void Drink()
    {
        Debug.Log(buffDuration);
        if (Time.time >= nextDrinkTime)
        {
            nextDrinkTime = Time.time + potionDrinkCooldown;
            Player.localPlayer.weaponSlot.AddBuff(new WeaponSlot.WeaponBuff { type = effectType, statusEffectDuration = statusEffectDuration, endTime = Time.time + buffDuration }, buffDuration);
            Player.localPlayer.dwarfAnimController.Drink();
        }
    }

    private string GetPotionName()
    {
        switch (effectType)
        {
            case StatusEffects.StatusEffectType.Bleed:

                return "Potion of Bleeding";
            case StatusEffects.StatusEffectType.Slow:

                return "Potion of Slowness";
            case StatusEffects.StatusEffectType.ArmorBreak:

                return "Potion of Broken Armor";
            case StatusEffects.StatusEffectType.WeaponBreak:

                return "Potion of Broken Weapons";
            case StatusEffects.StatusEffectType.Poison:

                return "Potion of Poison";
            default:

                return "StatusEffectType Not Found! (Potion.GetPotionName())";
        }
    }

    private float GetBuffDuration()
    {
        switch (itemRarity)
        {
            case ItemRarity.common:

                return 4f;
            case ItemRarity.rare:

                return 5.5f;
            case ItemRarity.epic:

                return 7f;
            case ItemRarity.legendary:

                return 10f;
            default:

                return buffDuration;
        }
    }

    private float GetStatusEffectDuration()
    {
        switch (effectType)
        {
            case StatusEffects.StatusEffectType.Bleed:

                return 3f;
            case StatusEffects.StatusEffectType.Slow:

                return 2f;
            case StatusEffects.StatusEffectType.ArmorBreak:

                return 5f;
            case StatusEffects.StatusEffectType.WeaponBreak:

                return 4f;
            case StatusEffects.StatusEffectType.Poison:

                return 3f;
            default:

                return statusEffectDuration;
        }
    }
}
