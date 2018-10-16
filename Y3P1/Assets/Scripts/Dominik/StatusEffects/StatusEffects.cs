using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffects
{

    private Entity myEntity;
    public static float tickRate = 1f;
    private float nextTick;
    public enum StatusEffectType { Bleed, Slow, ArmorBreak, WeaponBreak };

    public event Action<StatusEffectType> OnEffectStarted = delegate { };
    public event Action<StatusEffectType> OnEffectEnded = delegate { };

    private List<StatusEffect> activeEffects = new List<StatusEffect>();

    public void Initialise(Entity entity)
    {
        myEntity = entity;
    }

    public void ApplyWeaponBuffs(List<WeaponSlot.WeaponBuff> weaponBuffs)
    {
        for (int i = 0; i < weaponBuffs.Count; i++)
        {
            myEntity.photonView.RPC("SyncStatusEffects", RpcTarget.All, (int)weaponBuffs[i].type, weaponBuffs[i].statusEffectDuration);
        }
    }

    public void AddEffect(int effectType, float duration)
    {
        StatusEffectType type = (StatusEffectType)effectType;

        for (int i = 0; i < activeEffects.Count; i++)
        {
            if (activeEffects[i].type == type)
            {
                activeEffects[i].Refresh();
                return;
            }
        }

        StatusEffect newEffect = null;

        switch (type)
        {
            case StatusEffectType.Bleed:

                newEffect = new StatusEffect_Burn();
                break;
            case StatusEffectType.Slow:

                newEffect = new StatusEffect_Slow();
                break;
            case StatusEffectType.ArmorBreak:

                newEffect = new StatusEffect_ArmorBreak();
                break;
            case StatusEffectType.WeaponBreak:

                newEffect = new StatusEffect_WeaponBreak();
                break;
        }

        newEffect.Initialise(myEntity, duration);
        activeEffects.Add(newEffect);
        OnEffectStarted(type);
    }

    public void HandleEffects()
    {
        if (Time.time >= nextTick)
        {
            nextTick = Time.time + tickRate;
            TriggerEffects();
        }
    }

    public void TriggerEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            if (!activeEffects[i].HasEnded())
            {
                activeEffects[i].TriggerEffect();
            }
            else
            {
                OnEffectEnded(activeEffects[i].type);
                activeEffects[i].EndEffect();
                activeEffects.Remove(activeEffects[i]);
            }
        }
    }
}
