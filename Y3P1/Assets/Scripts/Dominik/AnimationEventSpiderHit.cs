using UnityEngine;

public class AnimationEventSpiderHit : MonoBehaviour
{

    private AI ai;

    private void Awake()
    {
        ai = GetComponentInParent<AI>();
    }

    public void HitEnd()
    {
        ai.ResetCanMove();
    }
}
