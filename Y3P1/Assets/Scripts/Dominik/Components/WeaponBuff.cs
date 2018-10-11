using Photon.Pun;
using UnityEngine;
using Y3P1;

public class WeaponBuff : MonoBehaviourPunCallbacks
{

    private Projectile projectile;

    [SerializeField] private string projecileToSpawnOnAttack;

    private void Awake()
    {
        projectile = GetComponent<Projectile>();
    }

    public override void OnEnable()
    {
        WeaponSlot.OnUsePrimary += WeaponSlot_OnUsePrimary;
    }

    private void WeaponSlot_OnUsePrimary()
    {
        WeaponPrefab wp = Player.localPlayer.weaponSlot.equipedItem.GetComponent<WeaponPrefab>();
        wp.FireProjectile(wp.projectileSpawn.position, Player.localPlayer.playerController.body.transform.rotation, projecileToSpawnOnAttack, 7, projectile.fireData.damage);
    }

    public override void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
    }
}
