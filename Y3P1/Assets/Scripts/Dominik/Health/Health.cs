using System;
using UnityEngine;

[Serializable]
public class Health
{

    private Entity myEntity;

    public bool isImmortal;
    public bool isInvinsible;
    private bool isDead;
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
        if (amount >= 0)
        {
            currentHealth += amount;
        }
        else
        {
            if (!isImmortal && !isInvinsible)
            {
                currentHealth += amount;
            }
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        OnHealthModified(GetHealthPercentage(), amount);

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            myEntity.Kill();
        }
    }

    private float GetHealthPercentage()
    {
        return (float)currentHealth / maxHealth;
    }

    public string GetHealthString()
    {
        return currentHealth + "/" + maxHealth;
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = maxHealth;
        OnHealthModified(GetHealthPercentage(), null);
    }
}
