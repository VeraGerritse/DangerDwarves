using Photon.Pun;
using UnityEngine;
using Y3P1;

public class WeaponBuff_OLD : MonoBehaviourPunCallbacks
{

    private Projectile projectile;
    private Vector3 spawnPosition;
    private Quaternion spawnRotation;
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

        spawnPosition = spawnAtCursor ? PlayerController.mouseInWorldPos : wp.projectileSpawn.position;
        spawnPosition += spawnOffset;
        spawnRotation = projectileRotation == ProjectileRotation.Player ? Player.localPlayer.playerController.body.transform.rotation : Quaternion.identity;
        spawnRotation.eulerAngles += spawnRotOffset;

        ProjectileManager.instance.FireProjectile(new ProjectileManager.ProjectileData
        {
            spawnPosition = spawnPosition,
            spawnRotation = spawnRotation,
            projectilePool = projecileToSpawnOnAttack,
            speed = speed,
            damage = projectile.fireData.damage
        });
    }

    public override void OnDisable()
    {
        WeaponSlot.OnUsePrimary -= WeaponSlot_OnUsePrimary;
    }
}
