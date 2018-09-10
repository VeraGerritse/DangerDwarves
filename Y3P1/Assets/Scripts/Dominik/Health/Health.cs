using System;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviourPunCallbacks
{

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public event Action<float> OnHealthModified = delegate { };
    public event Action OnDeath = delegate { };

    public override void OnEnable()
    {
        base.OnEnable();

        ResetHealth();
    }

    public void ModifyHealth(int amount)
    {
        photonView.RPC("ModifyHealthRPC", RpcTarget.AllBuffered, amount);
    }

    [PunRPC]
    private void ModifyHealthRPC(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthModified(GetHealthPercentage());

        if (currentHealth <= 0)
        {
            Kill();
        }
    }

    private float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthModified(GetHealthPercentage());
    }

    private void Kill()
    {
        print(gameObject.name + " has died");
        OnDeath();
    }
}
