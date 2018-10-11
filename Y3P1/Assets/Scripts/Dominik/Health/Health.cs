using System;
using UnityEngine;

[Serializable]
public class Health
{

    private Entity myEntity;

    public bool isImmortal;
    public bool isInvinsible;
    public bool isDead;

    [SerializeField] private int baseHealth = 100;
    [HideInInspector] public int currentHealth;

    public event Action<HealthData> OnHealthModified = delegate { };

    public struct HealthData
    {
        public int currentHealth;
        public int maxHealth;
        public float percentageHealth;
        public int? amountHealthChanged;
    }

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
        currentHealth = Mathf.Clamp(currentHealth, 0, GetMaxHealth());
        OnHealthModified(new HealthData { currentHealth =  currentHealth, maxHealth = GetMaxHealth(), percentageHealth = GetHealthPercentage(), amountHealthChanged = amount});

        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            myEntity.Kill();
        }
    }

    public float GetHealthPercentage()
    {
        return (float)currentHealth / GetMaxHealth();
    }

    public string GetHealthString()
    {
        return currentHealth + "/" + GetMaxHealth();
    }

    private int GetMaxHealth()
    {
        if (myEntity)
        {
            return baseHealth + myEntity.stats.stamina * 5;
        }

        return baseHealth;
    }

    public void UpdateHealth()
    {
        currentHealth = Mathf.Clamp(currentHealth, 0, GetMaxHealth());
        OnHealthModified(new HealthData { currentHealth = currentHealth, maxHealth = GetMaxHealth(), percentageHealth = GetHealthPercentage(), amountHealthChanged = null });
    }

    public void ResetHealth()
    {
        isDead = false;
        currentHealth = GetMaxHealth();
        OnHealthModified(new HealthData { currentHealth = currentHealth, maxHealth = GetMaxHealth(), percentageHealth = GetHealthPercentage(), amountHealthChanged = null });
    }
}
