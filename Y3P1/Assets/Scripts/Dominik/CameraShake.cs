using UnityEngine;

public class CameraShake : MonoBehaviour
{

    private float time;
    private Vector3 nextShakePos;

    public bool canShake = true;

    [Space(10)]

    [Range(0, 1)] [SerializeField] private float trauma;
    [SerializeField] private float force = 10f;
    [SerializeField] private float magnitude = 1f;
    [SerializeField] private float rotationalMagnitude;
    [SerializeField] private float depthMagnitude;
    [SerializeField] private float falloff = 1f;

    public float Trauma
    {
        get { return trauma; }
        set { trauma = Mathf.Clamp01(value); }
    }

    private void LateUpdate()
    {
        if (!canShake)
        {
            return;
        }

        if (trauma > 0.01f)
        {
            time += Time.deltaTime * trauma * force;

            nextShakePos = GetNewPosition() * magnitude * trauma;
            transform.localPosition = nextShakePos;
            transform.localRotation = Quaternion.Euler(nextShakePos * rotationalMagnitude);

            trauma -= Time.deltaTime * falloff * trauma;
        }
        else
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.Euler(Vector3.zero);
        }
    }

    private Vector3 GetNewPosition()
    {
        return new Vector3
        (
            (Mathf.PerlinNoise(10, time) - 0.5f) * 2,
            (Mathf.PerlinNoise(50, time) - 0.5f) * 2,
            (Mathf.PerlinNoise(90, time) - 0.5f) * 2 * depthMagnitude
        );
    }
}
