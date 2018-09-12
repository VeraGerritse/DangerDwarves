using System;
using UnityEngine;
using Photon.Pun;

[Serializable]
public class Health
{

    private Entity myEntity;

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public event Action<float, int?> OnHealthModified = delegate { };

    public void Initialise(Entity entity)
    {
        myEntity = entity;
        ResetHealth();
    }

    public void ModifyHealth(int amount)
    {
        //myEntity.photonView.RPC("ModifyHealthRPC", RpcTarget.AllBuffered, amount);
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthModified(GetHealthPercentage(), amount);

        if (currentHealth <= 0)
        {
            myEntity.Kill();
        }
    }

    [PunRPC]
    private void ModifyHealthRPC(int amount)
    {
    }

    private float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public void ResetHealth()
    {
        currentHealth = maxHealth;
        OnHealthModified(GetHealthPercentage(), null);
    }
}
