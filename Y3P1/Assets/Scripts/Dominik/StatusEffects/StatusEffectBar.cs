using System.Collections.Generic;
using UnityEngine;

public class StatusEffectBar : MonoBehaviour
{

    private bool initialised;
    private Entity myEntity;

    [SerializeField] private List<StatusEffectIcon> statusEffectIcons = new List<StatusEffectIcon>();

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
        for (int i = 0; i < statusEffectIcons.Count; i++)
        {
            if (statusEffectIcons[i].type == type)
            {
                if (toggle)
                {
                    statusEffectIcons[i].Activate();
                }
                else
                {
                    statusEffectIcons[i].gameObject.SetActive(false);
                }
            }
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
