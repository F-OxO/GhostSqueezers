using TMPro;
using UnityEngine;

public class HUD : MonoBehaviour {

    [SerializeField] private PolarBeltProvider polarBeltProvider;

    [SerializeField] private TextMeshProUGUI textHeartRate;
    [SerializeField] private TextMeshProUGUI textRRInterval;

    private bool _initialized;

    private bool _valuesDirty = false;
    private volatile int _heartRate;
    private volatile float _rrInterval;



    private void Update() {

        if (!_initialized && polarBeltProvider.PolarBelt != null) {
            polarBeltProvider.PolarBelt.OnHeartRateChanged += PolarBelt_OnHeartRateChanged;
            polarBeltProvider.PolarBelt.OnRRIntervalChanged += PolarBelt_OnRRIntervalChanged;
            _initialized = true;
        }

        if (_valuesDirty) {
            textHeartRate.text = $"{_heartRate} BPM";
            textRRInterval.text = $"RR: {_rrInterval:F2}ms";
        }
    }

    private void PolarBelt_OnRRIntervalChanged(float rrInterval) {
        _rrInterval = rrInterval;
        _valuesDirty = true;
    }

    private void PolarBelt_OnHeartRateChanged(int heartRate) {
        _heartRate = heartRate;
        _valuesDirty = true;
    }
}
