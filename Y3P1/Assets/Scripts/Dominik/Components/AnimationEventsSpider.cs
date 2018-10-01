using UnityEngine;

public class AnimationEventsSpider : MonoBehaviour
{

    private AI ai;

    private void Awake()
    {
        ai = GetComponentInParent<AI>();
    }

    public void AttackEnd()
    {
        ai.EndAttack();
    }

    public void Attack()
    {
        ai.Attack();
    }

    public void HitEnd()
    {
        ai.HitEnd();
    }
}
