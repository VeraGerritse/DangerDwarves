using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    public Health health;
    public Stats stats;
    public bool canDropLoot;
    public event Action OnDeath = delegate { };

    [Space(10)]

    public UnityEvent OnHitEvent;

    public override void OnEnable()
    {
        health.Initialise(this);
    }

    public void Hit(int amount)
    {
        if (photonView.IsMine)
        {
            OnHitEvent.Invoke();
            photonView.RPC("HitRPC", RpcTarget.AllBuffered, amount);
        }
    }

    [PunRPC]
    private void HitRPC(int amount)
    {
        health.ModifyHealth(CalculateAmount(amount));
    }

    private int CalculateAmount(int amount)
    {
        // Heals dont get affected by stats.
        if (amount > 0)
        {
            return amount;
        }

        return (int)Mathf.Clamp((amount + stats.defense), -99999999999999999, 0);
    }

    public void UpdateStats(Stats stats)
    {
        this.stats = stats;
        health.UpdateHealth();
    }

    public int CalculateDamage(Weapon.DamageType damageType)
    {
        switch (damageType)
        {
            case Weapon.DamageType.Melee:

                return stats.strength + (int)(0.2 * stats.agility);
            case Weapon.DamageType.Ranged:

                return stats.agility + (int)(0.2 * stats.strength);
            case Weapon.DamageType.Secondary:

                return stats.willpower + (int)(0.5 * stats.strength) + (int)(0.5 * stats.agility);
        }

        return 0;
    }

    public void Kill()
    {
        OnDeath();
        EntityManager.instance.RemoveFromAliveTargets(this);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health.isImmortal);
        }
        else
        {
            health.isImmortal = (bool)stream.ReceiveNext();
        }
    }
}
