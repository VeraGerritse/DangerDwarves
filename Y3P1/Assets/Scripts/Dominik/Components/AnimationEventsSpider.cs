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

    public void DestroySpider()
    {
        if (Photon.Pun.PhotonNetwork.IsMasterClient)
        {
            Photon.Pun.PhotonNetwork.Destroy(ai.gameObject);
        }
    }
}
