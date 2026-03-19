using UnityEngine;
using System;

public class Doll : MonoBehaviour {

    public enum EEmgValueType {
        raw, absolute
    }

    [SerializeField] private MyoArmbandProvider dataSource;

    [SerializeField] private EEmgValueType valueType = EEmgValueType.raw;
    [SerializeField] private GameObject beam;
    [SerializeField] private GameObject aura;
    [SerializeField] private RayCaster raycaster;
    [SerializeField] private float multiplier = 1.5f;

    private float[] moving_avg = new float[16];
    int moving_idx = 0;


    private void Awake() {
        if (dataSource != null) {
            dataSource.OnEMGSampleReceived += ReceiveDataSample;
        }
    }

    private void OnDestroy() {
        if (dataSource != null) {
            dataSource.OnEMGSampleReceived -= ReceiveDataSample;
        }
    }

    private void Start() {
        ReceiveDataSample(new sbyte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
    }

    private void Update()
    {

    }

    private void ReceiveDataSample(sbyte[] sample) {
        float avg_sample = 0f;
        for (int i = 0; i < 8; i++) {                   
            float normalizedValue = sample[i] / 128f;
            avg_sample += Math.Abs(normalizedValue);
        }

        moving_avg[moving_idx] = avg_sample / 8f;
        moving_idx = (moving_idx + 1) % moving_avg.Length;

        float avg = 0f;
        for (int i = 0; i < moving_avg.Length; i++) {                
            avg += moving_avg[i];
        }
        avg /= moving_avg.Length;

        float value = 0f;
        if(avg >= 0.08f)
        {
            value = Math.Min(1f, avg * multiplier);
        }

        Ray ray = new Ray(beam.transform.position, beam.transform.up);
        float rayDist = raycaster.CastRay(ray, value * 10f);

        beam.transform.localScale = new Vector3(value, rayDist, value);

        float max_aura = 0.33f;
        float aura_scale = max_aura - value * max_aura;
        aura.transform.localScale = new Vector3(aura_scale, aura_scale, aura_scale);
    }
}
