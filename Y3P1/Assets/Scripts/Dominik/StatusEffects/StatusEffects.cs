using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class StatusEffects
{

    private Entity myEntity;
    public static float tickRate = 1f;
    private float nextTick;
    public enum StatusEffectType { Burn };

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
            case StatusEffectType.Burn:

                newEffect = new StatusEffect_Burn();
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
                activeEffects.Remove(activeEffects[i]);
            }
        }
    }

    public abstract class StatusEffect
    {
        protected Entity entity;
        public StatusEffectType type;
        public float duration;
        private float remainingDuration;

        public virtual void Initialise(Entity entity, float duration)
        {
            this.entity = entity;

            this.duration = duration;
            remainingDuration = duration;
        }

        public virtual void TriggerEffect()
        {
            remainingDuration -= StatusEffects.tickRate;
        }

        public void Refresh()
        {
            remainingDuration = duration;
        }

        public bool HasEnded()
        {
            return remainingDuration <= 0;
        }
    }

    public class StatusEffect_Burn : StatusEffect
    {
        public int damage = 5;

        public override void TriggerEffect()
        {
            base.TriggerEffect();

            if (PhotonNetwork.IsMasterClient)
            {
                entity.Hit(-damage);
            }
        }
    }
}
