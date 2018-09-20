using System.Collections.Generic;
using UnityEngine;

public class Projectile_Animated : Projectile
{

    [SerializeField] private List<Projectile> animatedProjectiles = new List<Projectile>();

    public override void Awake()
    {
        base.Awake();

        for (int i = 0; i < animatedProjectiles.Count; i++)
        {
            animatedProjectiles[i].OnEntityHit += Projectile_Animated_OnEntityHit;
        }
    }

    public override void Fire(float speed, int damage, int targetID = 9999)
    {
        base.Fire(speed, damage);

        for (int i = 0; i < animatedProjectiles.Count; i++)
        {
            animatedProjectiles[i].gameObject.SetActive(true);
            animatedProjectiles[i].Fire(moveSpeed, damage);
        }
    }

    private void Projectile_Animated_OnEntityHit(Projectile projectile)
    {
        projectile.gameObject.SetActive(false);
    }
}
