using UnityEngine;

public class BarChart2D : MonoBehaviour {

    public enum EEmgValueType {
        raw, absolute
    }

    [SerializeField] private MyoArmbandProvider dataSource;

    [SerializeField] private EEmgValueType valueType = EEmgValueType.raw;

    [SerializeField] private Transform[] innerBars;

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
        ReceiveDataSample(new sbyte[] { -128, -100, -50, -20, 0, 50, 80, 127 });
    }

    private void ReceiveDataSample(sbyte[] sample) {
        for (int i = 0; i < innerBars.Length; i++) {

            // Normalize signed byte (-128 to 127) into [0, 1]                        
            float normalizedValue = (sample[i] + 128f) / 256f;

            //Set Bar height from data sample
            if (valueType == EEmgValueType.raw) {
                SetBarHeightRaw(innerBars[i], normalizedValue);
            } else if (valueType == EEmgValueType.absolute) {
                SetBarHeightAbsolute(innerBars[i], normalizedValue);
            }
        }
    }

    private void SetBarHeightRaw(Transform bar, float normalizedValue) {
        Vector3 scale = bar.localScale;
        scale.y = normalizedValue;
        bar.localScale = scale;

        Vector3 pos = bar.localPosition;
        pos.y = (1f - normalizedValue) / 2f;
        bar.localPosition = -pos;
    }

    private void SetBarHeightAbsolute(Transform bar, float normalizedValue) {
        float absoluteValue;
        if (normalizedValue > 0.5f) {
            absoluteValue = (normalizedValue - 0.5f) * 2f;
        } else if (normalizedValue < 0.5f) {
            absoluteValue = (0.5f - normalizedValue) * 2f;
        } else {
            absoluteValue = 0f;
        }

        Vector3 scale = bar.localScale;
        scale.y = absoluteValue;
        bar.localScale = scale;

        Vector3 pos = bar.localPosition;
        pos.y = 0;
        bar.localPosition = -pos;
    }
}
