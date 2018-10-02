using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using System;

public class Entity : MonoBehaviourPunCallbacks, IPunObservable
{

    public Health health;
    public Stats stats;
    public event Action OnDeath = delegate { };

    [Space(10)]

    public UnityEvent OnHitEvent;

    public override void OnEnable()
    {
        health.Initialise(this);
    }

    public void Hit(int amount)
    {
        OnHitEvent.Invoke();
        photonView.RPC("HitRPC", RpcTarget.AllBuffered, amount);
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
