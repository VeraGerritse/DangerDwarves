using System.Collections.Generic;
using UnityEngine;

public class Projectile_Animated : Projectile
{

    private int hitProjectiles;

    [SerializeField] private List<Projectile> animatedProjectiles = new List<Projectile>();

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < animatedProjectiles.Count; i++)
        {
            animatedProjectiles[i].OnEntityHit += OnHitAnything;
            animatedProjectiles[i].OnEnvironmentHit += OnHitAnything;
        }
    }

    public override void Fire(FireData fireData)
    {
        base.Fire(fireData);

        for (int i = 0; i < animatedProjectiles.Count; i++)
        {
            animatedProjectiles[i].gameObject.SetActive(true);
            animatedProjectiles[i].Fire(fireData);
        }
    }

    private void OnHitAnything(Projectile projectile)
    {
        RegisterHit();
    }

    private void RegisterHit()
    {
        hitProjectiles++;
        if (hitProjectiles == animatedProjectiles.Count)
        {
            ReturnToPool();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        hitProjectiles = 0;
    }
}
