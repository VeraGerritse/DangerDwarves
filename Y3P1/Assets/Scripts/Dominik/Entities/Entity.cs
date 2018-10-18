using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Y3P1;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    [HideInInspector] public Collider myCollider;

    [SerializeField] private int entityID;
    [SerializeField] private bool instaDestroyOnDeath;
    [SerializeField] private Rigidbody rb;
    public enum EntityType { Humanoid, Prop, Chest, Box };
    [SerializeField] private EntityType entityType;

    [Space(10)]

    public Health health;
    public Stats stats;
    public StatusEffects statusEffects = new StatusEffects();

    [Space(10)]

    public bool canDropLoot;
    [SerializeField] private bool scaleWithWorld;

    [Space(10)]

    public UnityEvent OnHit;
    public UnityEvent OnDeath;
    public UnityEvent OnRevive;

    private void Awake()
    {
        myCollider = GetComponent<Collider>();
    }

    public override void OnEnable()
    {
        health.Initialise(this);
        statusEffects.Initialise(this);

        if (scaleWithWorld)
        {
            LevelStats(Player.localPlayer.myInventory.aIL.averageILevel);
            health.LevelHealth(Player.localPlayer.myInventory.aIL.averageILevel);
        }
    }

    private void Update()
    {
        statusEffects.HandleEffects();
    }

    public void Hit(int amount, Stats.DamageType damageType, List<WeaponSlot.WeaponBuff> weaponBuffs = null)
    {
        photonView.RPC("HitRPC", RpcTarget.All, CalculateAmount(amount), health.isInvinsible, (int)damageType);

        if (weaponBuffs != null)
        {
            statusEffects.ApplyWeaponBuffs(weaponBuffs);
        }
    }

    [PunRPC]
    private void HitRPC(int amount, bool isInvinsible, int damageType)
    {
        if (amount <= 0 && !isInvinsible)
        {
            OnHit.Invoke();
        }
        health.ModifyHealth(amount, (Stats.DamageType)damageType);
    }

    [PunRPC]
    public void SyncStatusEffects(int effectType, float duration, int value = -1)
    {
        statusEffects.AddEffect(effectType, duration, value);
    }

    public void LevelStats(float aIL)
    {
        float multiplier = aIL / 10 + 1 - 0.1f;
        stats.agility = Mathf.RoundToInt(stats.agility * multiplier);
        stats.agility += Random.Range(0, 3);

        stats.willpower = Mathf.RoundToInt(stats.willpower * multiplier);
        stats.willpower += Random.Range(0, 3);

        stats.strength = Mathf.RoundToInt(stats.strength * multiplier);
        stats.strength += Random.Range(0, 3);

        stats.stamina = Mathf.RoundToInt(stats.stamina * multiplier);
        stats.stamina += Random.Range(0, 3);

        stats.defense = Mathf.RoundToInt(stats.defense * multiplier);
        stats.defense += Random.Range(0, 3);
    }

    public void UpdateStats(Stats stats)
    {
        this.stats = stats;
        health.UpdateHealth();
        //TODO: This syncs one update too late.
        photonView.RPC("SyncHealth", RpcTarget.Others);
    }

    private int CalculateAmount(int amount)
    {
        // Heals dont get affected by stats.
        if (amount > 0)
        {
            return amount;
        }

        float incomingDamage = Mathf.Abs(amount);
        float damageCoeff = Mathf.Min(0.99f, Mathf.Pow(incomingDamage / (stats.DefenseEffectiveness * stats.defense * 0.15f), 0.2f));
        float reducedDamage = damageCoeff * incomingDamage;

        return (int)Mathf.Clamp(-reducedDamage, -99999999999999999, -1);
    }

    public int CalculateDamage(Stats.DamageType damageType)
    {
        switch (damageType)
        {
            case Stats.DamageType.Melee:

                return (int)(stats.DamageEffectiveness * (stats.strength + (int)(0.2 * stats.agility)));
            case Stats.DamageType.Ranged:

                return (int)(stats.DamageEffectiveness * (stats.agility + (int)(0.2 * stats.strength)));
            case Stats.DamageType.Secondary:

                return (int)(stats.DamageEffectiveness * (stats.willpower * 2 + (int)(0.5 * stats.strength) + (int)(0.5 * stats.agility)));
        }

        return 0;
    }

    public void Kill()
    {
        OnDeath.Invoke();
        EntityManager.instance.RemoveFromAliveTargets(this);
        BountyManager.instance.RegisterKill(entityID);

        if (instaDestroyOnDeath)
        {
            DestroyEntity();
        }
    }

    public void DestroyEntity()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Destroy(transform.root.gameObject);
        }
    }

    public void Revive(int healthPercentage)
    {
        OnRevive.Invoke();
        photonView.RPC("SyncReviveHealth", RpcTarget.All, healthPercentage);
    }

    [PunRPC]
    private void SyncReviveHealth(int percentage)
    {
        health.ResetHealth(percentage);
    }

    [PunRPC]
    private void SyncHealth()
    {
        health.UpdateHealth();
    }

    public void KnockBack(Vector3 direction, float force)
    {
        if (!rb)
        {
            return;
        }

        photonView.RPC("KnockBackRPC", RpcTarget.All, direction, force);
    }

    [PunRPC]
    private void KnockBackRPC(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

    private void OnDestroy()
    {
        if (PhotonNetwork.IsMasterClient && canDropLoot)
        {
            Player.localPlayer.myInventory.DropNewItem(transform.position,entityType);
        }
    }

    //public override void OnPlayerEnteredRoom(Player newPlayer)
    //{
    //    photonView.RPC("SyncHealth", RpcTarget.Others);
    //}

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health.isImmortal);
            stream.SendNext(health.isInvinsible);
            stream.SendNext(health.isDead);
            stream.SendNext(health.currentHealth);

            stream.SendNext(stats.DefenseEffectiveness);
            stream.SendNext(stats.DamageEffectiveness);

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

            stats.DefenseEffectiveness = (float)stream.ReceiveNext();
            stats.DamageEffectiveness = (float)stream.ReceiveNext();

            stats.stamina = (int)stream.ReceiveNext();
            stats.strength = (int)stream.ReceiveNext();
            stats.agility = (int)stream.ReceiveNext();
            stats.willpower = (int)stream.ReceiveNext();
            stats.defense = (int)stream.ReceiveNext();
        }
    }
}
