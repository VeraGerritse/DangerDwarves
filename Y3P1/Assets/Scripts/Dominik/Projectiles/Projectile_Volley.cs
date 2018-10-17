using UnityEngine;

public class Projectile_Volley : Projectile
{

    private Vector3 volleyArrowSpawnRotation = new Vector3(90, 0, 0);

    [Header("Volley Settings")]
    [SerializeField] private string volleyArrowPrefab;
    [SerializeField] private float volleyRange;
    [SerializeField] private int volleyAmount;
    [SerializeField] private int volleyArrowSpeed;

    private void SpawnVolley()
    {
        for (int i = 0; i < volleyAmount; i++)
        {
            Vector3 randomPos = transform.position + Random.insideUnitSphere * volleyRange;
            randomPos.y += 20;

            Projectile projectile = ObjectPooler.instance.GrabFromPool(volleyArrowPrefab, randomPos, Quaternion.Euler(volleyArrowSpawnRotation)).GetComponent<Projectile>();
            projectile.Fire(new FireData
            {
                speed = Random.Range(0.8f * volleyArrowSpeed, 1.2f * volleyArrowSpeed),
                damage = fireData.damage,
                ownerID = fireData.ownerID
            });
        }
    }

    public override void OnDisable()
    {
        if (hitAnything)
        {
            SpawnVolley();
        }

        base.OnDisable();
    }
}
