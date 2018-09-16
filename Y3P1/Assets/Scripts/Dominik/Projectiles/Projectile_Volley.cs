using UnityEngine;

public class Projectile_Volley : Projectile 
{

    private Vector3 volleyArrowSpawnRotation = new Vector3(90, 0, 0);

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
            print(randomPos);

            Projectile projectile = ObjectPooler.instance.GrabFromPool(volleyArrowPrefab, randomPos, Quaternion.Euler(volleyArrowSpawnRotation)).GetComponent<Projectile>();
            projectile.Fire(Random.Range(0.8f * volleyArrowSpeed, 1.2f * volleyArrowSpeed), damage);
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        if (hitEntity)
        {
            SpawnVolley();
        }
    }
}
