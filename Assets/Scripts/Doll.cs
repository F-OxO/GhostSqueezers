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

    private void ReceiveDataSample(sbyte[] sample) {
        float avg = 0;
        for (int i = 0; i < 8; i++) {                   
            float normalizedValue = sample[i] / 128f;
            avg += Math.Abs(normalizedValue);
        }

        beam.transform.localScale = new Vector3(avg / 8f, avg / 8f, avg / 8f);
    }
}
