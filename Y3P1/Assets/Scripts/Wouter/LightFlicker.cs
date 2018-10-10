using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{

    private Light myLight;

    public float minIntensity;
    public float maxIntensity;

    public float minRange;
    public float maxRange;

    private float random;

    private void Awake()
    {
        myLight = GetComponent<Light>();
    }

    private void Start()
    {
        random = Random.Range(0, 1000);
    }

    private void Update()
    {
        float noise = Mathf.PerlinNoise(random, Time.time);
        myLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, noise);
        myLight.range = Mathf.Lerp(minRange, maxRange, noise);
    }
}
