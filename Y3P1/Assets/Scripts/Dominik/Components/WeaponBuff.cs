using Photon.Pun;
using UnityEngine;
using Y3P1;

public class WeaponBuff : MonoBehaviourPunCallbacks
{

    private Projectile projectile;
    private Vector3 spawnPos;
    private Quaternion spawnRot;
    private enum ProjectileRotation { Identity, Player };

    [SerializeField] private string projecileToSpawnOnAttack;
    [SerializeField] private bool spawnAtCursor;
    [SerializeField] private Vector3 spawnOffset;
    [SerializeField] private Vector3 spawnRotOffset;
    [SerializeField] private float speed;
    [SerializeField] private ProjectileRotation projectileRotation;

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

        spawnPos = spawnAtCursor ? PlayerController.mouseInWorldPos : wp.projectileSpawn.position;
        spawnPos += spawnOffset;
        spawnRot = projectileRotation == ProjectileRotation.Player ? Player.localPlayer.playerController.body.transform.rotation : Quaternion.identity;
        spawnRot.eulerAngles += spawnRotOffset;

        wp.FireProjectile(spawnPos, spawnRot, projecileToSpawnOnAttack, speed, projectile.fireData.damage);
    }

    public override void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
    }
}
