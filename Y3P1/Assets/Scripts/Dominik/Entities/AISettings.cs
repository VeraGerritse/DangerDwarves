using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AISettings")]
public class AISettings : ScriptableObject 
{

    [Header("Attacking")]
    public List<AttackAnimation> attacks = new List<AttackAnimation>();
    public float attackDistance;
    public float attackRangeLookAtSpeed;
    [Range(0, 100)] public float randomRangedAttackChance;
    public float randomRangedAttackInterval = 1f;
    public bool stopWhileAttacking;

    [Header("Damaging")]
    public float damageRange;
    public LayerMask damageLayerMask;

    [Header("Animation")]
    public string deathAnimation;
    public string walkAnimation;
    public string hitAnimation;

    [Header("Other")]
    public float wanderRadius = 6f;
    public float minIdleTime = 1f;
    public float maxIdleTime = 4f;

    [System.Serializable]
    public struct AttackAnimation
    {
        public string attackName;
        [Range(0, 100)]
        public int attackChance;
        public enum AttackType { Animation, Projectile };
        public AttackType attackType;
        public Stats.DamageType damageType;
        public string projectilePoolName;
    }

    public AttackAnimation GetRandomAttack()
    {
        int random = Random.Range(0, 101);

        for (int i = 0; i < attacks.Count; i++)
        {
            if (random < attacks[i].attackChance)
            {
                return attacks[i];
            }
        }

        return attacks[0];
    }

    public List<AttackAnimation> GetAllRangedAttacks()
    {
        List<AttackAnimation> rangedAttacks = new List<AttackAnimation>();

        for (int i = 0; i < attacks.Count; i++)
        {
            if (attacks[i].attackType == AttackAnimation.AttackType.Projectile)
            {
                rangedAttacks.Add(attacks[i]);
            }
        }

        return rangedAttacks;
    }
}
