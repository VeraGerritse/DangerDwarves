using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    [SerializeField] private int entityID;
    [SerializeField] private bool instaDestroyOnDeath;

    public Health health;
    public Stats stats;
    public bool canDropLoot;
    public event Action OnDeath = delegate { };
    public event Action OnRevive = delegate { };

    [Space(10)]

    public UnityEvent OnHit;

    public override void OnEnable()
    {
        health.Initialise(this);
    }

    public void Hit(int amount)
    {
        if (amount <= 0)
        {
            OnHit.Invoke();
        }
        photonView.RPC("HitRPC", RpcTarget.All, CalculateAmount(amount));
    }

    [PunRPC]
    private void HitRPC(int amount)
    {
        health.ModifyHealth(amount);
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
        BountyManager.instance.RegisterKill(entityID);

        if (PhotonNetwork.IsMasterClient && instaDestroyOnDeath)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }

    public void Revive()
    {
        OnRevive();
        photonView.RPC("SyncReviveHealth", RpcTarget.All);
    }

    [PunRPC]
    private void SyncReviveHealth()
    {
        health.ResetHealth();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health.isImmortal);
            stream.SendNext(health.isInvinsible);
            stream.SendNext(health.isDead);
            stream.SendNext(health.currentHealth);

            stream.SendNext(stats.stamina);
            stream.SendNext(stats.strength);
            stream.SendNext(stats.agility);
            stream.SendNext(stats.willpower);
            stream.SendNext(stats.defense);
        }
        else
        {
            health.isImmortal = (bool)stream.ReceiveNext();
            health.isInvinsible = (bool)stream.ReceiveNext();
            health.isDead = (bool)stream.ReceiveNext();
            health.currentHealth = (int)stream.ReceiveNext();

            stats.stamina = (int)stream.ReceiveNext();
            stats.strength = (int)stream.ReceiveNext();
            stats.agility = (int)stream.ReceiveNext();
            stats.willpower = (int)stream.ReceiveNext();
            stats.defense = (int)stream.ReceiveNext();
        }
    }
}
