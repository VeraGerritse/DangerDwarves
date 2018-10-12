using UnityEngine;
using Photon.Pun;

public class ProjectileManager : MonoBehaviourPunCallbacks 
{

    public static ProjectileManager instance;

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
}
