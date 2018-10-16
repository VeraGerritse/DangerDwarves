using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBar : MonoBehaviour 
{

    private bool initialised;
    private Entity myEntity;

    [SerializeField] private GameObject burnIcon;
    [SerializeField] private GameObject slowIcon;
    [SerializeField] private GameObject armorBreakIcon;
    [SerializeField] private GameObject weaponBreakIcon;

    private void Awake()
    {
        if (!initialised)
        {
            Initialise(transform.root.GetComponentInChildren<Entity>());
        }
    }

    public void Initialise(Entity entity)
    {
        if (entity)
        {
            myEntity = entity;

            myEntity.statusEffects.OnEffectStarted += StatusEffects_OnEffectStarted;
            myEntity.statusEffects.OnEffectEnded += StatusEffects_OnEffectEnded;
            initialised = true;
        }
    }

    private void StatusEffects_OnEffectStarted(StatusEffects.StatusEffectType type)
    {
        ToggleEffectIcon(type, true);
    }

    private void StatusEffects_OnEffectEnded(StatusEffects.StatusEffectType type)
    {
        ToggleEffectIcon(type, false);
    }

    private void ToggleEffectIcon(StatusEffects.StatusEffectType type, bool toggle)
    {
        switch (type)
        {
            case StatusEffects.StatusEffectType.Burn:

                burnIcon.SetActive(toggle);
                break;
        }
    }

    private void OnDisable()
    {
        if (initialised)
        {
            myEntity.statusEffects.OnEffectStarted -= StatusEffects_OnEffectStarted;
            myEntity.statusEffects.OnEffectEnded -= StatusEffects_OnEffectEnded;
        }
    }
}
