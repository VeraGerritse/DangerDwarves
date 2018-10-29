using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using Y3P1;

public class ProjectileManager : MonoBehaviourPunCallbacks
{

    public static ProjectileManager instance;

    [System.Serializable]
    public struct ProjectileSettings
    {
        public string projectilePool;
        public enum ProjectileSpawn { Weapon, Mouse, Player };
        public ProjectileSpawn projectileSpawn;
        public bool freezePosition;
        public float speedOverride;
    }
    [SerializeField] private List<ProjectileSettings> projectileSettings = new List<ProjectileSettings>();

    public class ProjectileData
    {
        public Vector3 spawnPosition;
        public Quaternion spawnRotation;
        public string projectilePool;
        public float speed;
        public int damage;
        public int amount = 1;
        public int coneOfFireInDegrees = 0;
        public Vector3 mousePos = new Vector3();
        public int projectileOwnerID = 9999;
    }

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else if (instance && instance != this)
        {
            Destroy(this);
        }
    }

    public void FireProjectile(ProjectileData data)
    {
        photonView.RPC("FireProjectileRPC", RpcTarget.All,
            data.spawnPosition,
            data.spawnRotation,
            data.projectilePool,
            data.speed,
            data.damage,
            data.amount,
            data.coneOfFireInDegrees,
            data.mousePos,
            data.projectileOwnerID);
    }

    [PunRPC]
    private void FireProjectileRPC(Vector3 position, Quaternion rotation, string projectilePoolName, float speed, int damage, int amountOfProjectiles = 1, int coneOfFireInDegrees = 0, Vector3 mousePos = new Vector3(), int ownerID = 9999)
    {
        // Firing in a straight line.
        if (coneOfFireInDegrees == 0)
        {
            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, rotation).GetComponent<Projectile>();
                newProjectile.Fire(new Projectile.FireData
                {
                    speed = speed,
                    damage = damage,
                    mousePos = mousePos,
                    ownerID = ownerID
                });
            }
        }
        // Evenly divide (multiple) projectiles within the cone of fire.
        else
        {
            float angleStep = coneOfFireInDegrees / amountOfProjectiles;
            float startingAngle = coneOfFireInDegrees / 2 - angleStep / 2;

            Vector3 rot = rotation.eulerAngles;
            rot.y -= startingAngle;

            for (int i = 0; i < amountOfProjectiles; i++)
            {
                Projectile newProjectile = ObjectPooler.instance.GrabFromPool(projectilePoolName, position, Quaternion.Euler(rot)).GetComponent<Projectile>();
                newProjectile.Fire(new Projectile.FireData
                {
                    speed = speed,
                    damage = damage,
                    mousePos = mousePos,
                    ownerID = ownerID
                });

                rot.y += angleStep;
            }
        }
    }

    public Vector3 GetProjectileSpawn(WeaponPrefab weaponPrefab, string projectilePool)
    {
        Vector3 pos = Vector3.zero;

        for (int i = 0; i < projectileSettings.Count; i++)
        {
            if (projectileSettings[i].projectilePool == projectilePool)
            {
                switch (projectileSettings[i].projectileSpawn)
                {
                    case ProjectileSettings.ProjectileSpawn.Weapon:

                        pos = weaponPrefab.projectileSpawn.position;
                        break;
                    case ProjectileSettings.ProjectileSpawn.Mouse:

                        pos = PlayerController.mouseInWorldPos;
                        break;
                    case ProjectileSettings.ProjectileSpawn.Player:

                        pos = Player.localPlayer.transform.position;
                        break;
                }
            }
        }

        return pos;
    }

    public float GetProjectileSpeed(float defaultSpeed, string projectilePool)
    {
        for (int i = 0; i < projectileSettings.Count; i++)
        {
            if (projectileSettings[i].projectilePool == projectilePool && projectileSettings[i].freezePosition)
            {
                if (projectileSettings[i].freezePosition)
                {
                    return 0;
                }

                if (projectileSettings[i].speedOverride != 0)
                {
                    return projectileSettings[i].speedOverride;
                }
            }
        }

        return defaultSpeed;
    }
}
