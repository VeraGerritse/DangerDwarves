using UnityEngine;

public class WeaponBuff : MonoBehaviour
{

    private float buffEndTime;

    public enum BuffType { None, Fire };
    public BuffType buffType;
    [SerializeField] private float buffDuration = 5f;

    private void Update()
    {
        if (WeaponSlot.currentBuff == this)
        {
            if (Time.time >= buffEndTime)
            {
                EndBuff();
            }
        }
    }

    private void ActivateBuff()
    {
        if (WeaponSlot.currentBuff)
        {
            WeaponSlot.currentBuff.EndBuff();
        }

        WeaponSlot.currentBuff = this;
        buffEndTime = Time.time + buffDuration;
    }

    public void EndBuff()
    {
        WeaponSlot.currentBuff = null;
    }
}
