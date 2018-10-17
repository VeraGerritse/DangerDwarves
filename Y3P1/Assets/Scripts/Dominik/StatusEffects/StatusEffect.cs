using Photon.Pun;
using UnityEngine.AI;
using UnityEngine;

public abstract class StatusEffect
{
    protected Entity entity;
    protected bool active;
    private float remainingDuration;

    public StatusEffects.StatusEffectType type;
    public float duration;

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

    public virtual void EndEffect()
    {

    }
}

public class StatusEffect_Burn : StatusEffect
{
    private Rigidbody rb;

    public int damage = 5;

    public override void Initialise(Entity entity, float duration)
    {
        base.Initialise(entity, duration);
        type = StatusEffects.StatusEffectType.Bleed;
        rb = entity.transform.parent.GetComponent<Rigidbody>();
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            if (rb)
            {
                if (rb.velocity != Vector3.zero)
                {
                    entity.Hit(-damage);
                }
            }
        }
    }
}

public class StatusEffect_Slow : StatusEffect
{
    private float preSlowMovespeed;
    private NavMeshAgent agent;
    private PlayerController playerController;

    public float slowPercentage = 50f;

    public override void Initialise(Entity entity, float duration)
    {
        base.Initialise(entity, duration);
        type = StatusEffects.StatusEffectType.Slow;

        agent = entity.transform.root.GetComponentInChildren<NavMeshAgent>();
        if (agent)
        {
            preSlowMovespeed = agent.speed;
            return;
        }

        playerController = entity.transform.parent.GetComponent<PlayerController>();
        if (playerController)
        {
            preSlowMovespeed = playerController.moveSpeed;
        }

        TriggerEffect();
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            if (!active)
            {
                if (agent)
                {
                    agent.speed = slowPercentage / 100 * agent.speed;
                }
                if (playerController)
                {
                    playerController.AdjustMoveSpeed(slowPercentage / 100 * playerController.moveSpeed);
                }

                active = true;
            }
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            if (agent)
            {
                agent.speed = preSlowMovespeed;
            }
            if (playerController)
            {
                playerController.AdjustMoveSpeed(preSlowMovespeed);
            }
        }
    }
}

public class StatusEffect_ArmorBreak : StatusEffect
{
    public float armorEffectiveness = 0.5f;

    public override void Initialise(Entity entity, float duration)
    {
        base.Initialise(entity, duration);
        type = StatusEffects.StatusEffectType.ArmorBreak;

        TriggerEffect();
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            entity.stats.DefenseEffectiveness = armorEffectiveness;
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            entity.stats.DefenseEffectiveness = 1;
        }
    }
}

public class StatusEffect_WeaponBreak : StatusEffect
{
    public float damageEffectiveness = 0.5f;

    public override void Initialise(Entity entity, float duration)
    {
        base.Initialise(entity, duration);
        type = StatusEffects.StatusEffectType.WeaponBreak;

        TriggerEffect();
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            entity.stats.DamageEffectiveness = damageEffectiveness;
        }
    }

    public override void EndEffect()
    {
        base.EndEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            entity.stats.DamageEffectiveness = 1;
        }
    }
}

public class StatusEffect_Poison : StatusEffect
{
    public int damage = 5;

    public override void Initialise(Entity entity, float duration)
    {
        base.Initialise(entity, duration);
        type = StatusEffects.StatusEffectType.Bleed;
    }

    public override void TriggerEffect()
    {
        base.TriggerEffect();

        if (PhotonNetwork.IsMasterClient)
        {
            entity.Hit(-damage);
        }
    }
}
