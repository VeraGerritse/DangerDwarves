using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Health))]
public class Hitbox : MonoBehaviour 
{

    private Health health;
    [SerializeField] private Stats stats;

    [Space(10)]

    [SerializeField] private UnityEvent OnHitEvent;

    private void Awake()
    {
        health = GetComponent<Health>();

        if (!stats)
        {
            stats = ScriptableObject.CreateInstance<Stats>();
            Debug.LogWarning("Created a new Stats scriptable object for " + transform.root.name + " because one was not assigned.");
        }
    }

    public void Hit(int amount)
    {
        OnHitEvent.Invoke();

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
}
