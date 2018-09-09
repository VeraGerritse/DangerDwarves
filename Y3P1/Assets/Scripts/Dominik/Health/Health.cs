using System;
using UnityEngine;
using Photon.Pun;

public class Health : MonoBehaviour, IPunObservable
{

    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    public event Action<float> OnHealthModified = delegate { };

    private void OnEnable()
    {
        ResetHealth();
    }

    public void ModifyHealth(int amount)
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
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(currentHealth);
        }
        else
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
