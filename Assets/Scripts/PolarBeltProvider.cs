using PlayBionic.PolarBeltAPI.Device;
using PlayBionic.PolarBeltAPI.Manager;
using System;
using System.Collections;
using UnityEngine;

public class PolarBeltProvider : MonoBehaviour {

    // TODO: move to API
    private enum EServiceState {
        off, starting, running
    }

    public PolarBelt PolarBelt { get; private set; }

    [SerializeField] private string deviceAddress = "24acac0db0f1"; // 24:ac:ac:0d:b0:f1

    private float _connectCooldown = 2f;
    private EServiceState _ecgServiceState = EServiceState.off; // TODO: Move to API

    private void Awake() {

        Application.wantsToQuit += Application_wantsToQuit;

        // Initialize the API
#if UNITY_EDITOR
        try {
            PolarBeltManager.initialize(new PlayBionic.PolarBeltAPI.Windows.WindowsAdapter("pb_polarbelt_bridge"));
        } catch (Exception) {
            Debug.LogWarning("Cannot initialize PolarBelt API");
        }
#elif UNITY_ANDROID && !UNITY_EDITOR      
        PolarBeltManager.initialize(new PlayBionic.PolarBeltAPI.Android.AndroidAdapter());        
#endif        
    }

    private bool Application_wantsToQuit() {
        // Disconnect the device
        if (PolarBelt.ConnectionState == PolarBelt.EConnectionState.Connected || PolarBelt.ConnectionState == PolarBelt.EConnectionState.Connecting) {
            if (_ecgServiceState != EServiceState.off) {
                PolarBelt.StopECG(); // TODO: move to API
            }
            PolarBelt.Disconnect();
        }

        // Shut down the API to free the actual bluetooth resources
        PolarBeltManager.shutDown();
        return true;
    }

    private void Update() {
        if (!PolarBeltManager.isInitialized()) { return; }


        if (_connectCooldown == 0) {
            // Initially create the device (this must not happen immediately after initializing the API manager)
            if (PolarBelt == null) {
                PolarBelt = PolarBeltManager.createPolarBelt("polarBelt", deviceAddress);
                PolarBelt.OnConnectionStateChanged += PolarBelt_OnConnectionStateChanged;
                PolarBelt.OnBatteryLevelChanged += PolarBelt_OnBatteryLevelChanged;
                PolarBelt.OnHeartRateChanged += PolarBelt_OnHeartRateChanged;
                PolarBelt.OnRRIntervalChanged += PolarBelt_OnRRIntervalChanged;
            }

            // Keep the polar belt connected
            if (PolarBelt.ConnectionState == PolarBelt.EConnectionState.Disconnected ||
                PolarBelt.ConnectionState == PolarBelt.EConnectionState.Failed) {
                Debug.Log("Connect Polar Belt...");
                PolarBelt.Connect();
            }
        } else if (_connectCooldown > 0) {
            _connectCooldown -= Time.deltaTime;
            if (_connectCooldown < 0) {
                _connectCooldown = 0;
            }
        }

        // Experimental (and not working yet): start ECG stream
        if (PolarBelt != null) {
            if (_ecgServiceState == EServiceState.off && PolarBelt.ConnectionState == PolarBelt.EConnectionState.Connected) {
                StartCoroutine(StartECGDelayed());
                _ecgServiceState = EServiceState.starting;
            }
        }
    }

    private void PolarBelt_OnRRIntervalChanged(float rrInterval) {
        Debug.Log($"RR-Interval: {rrInterval:F2} ms");
    }

    private void PolarBelt_OnHeartRateChanged(int bpm) {
        Debug.Log($"Heart Rate: {bpm} BPM");
    }

    private void PolarBelt_OnBatteryLevelChanged(byte batteryLevel) {
        Debug.Log($"PolarBelt Battery: {batteryLevel}%");
    }

    private void PolarBelt_OnConnectionStateChanged(PolarBelt.EConnectionState connectionState) {
        Debug.Log("PolarBelt connection state changed to " + connectionState);
        if (connectionState == PolarBelt.EConnectionState.Connected) {
            PolarBelt.UpdateBatteryLevel(); // TODO - move to API
        } else if (connectionState == PolarBelt.EConnectionState.Failed || connectionState == PolarBelt.EConnectionState.Disconnected) {
            _connectCooldown = 30; // wait for 30 seconds until reconnect
            _ecgServiceState = EServiceState.off; // TODO - move to API
        }
    }

    // Experimental (and not working yet)
    private IEnumerator StartECGDelayed() {

        // Wait for 50 ms
        yield return new WaitForSeconds(0.5f);

        //_polarBelt.StopECG();
        //_polarBelt.StopAccelerometer();

        // Wait for 50 ms
        yield return new WaitForSeconds(0.5f);

        //_polarBelt.StartECG();
        //_polarBelt.StartAccelerometer(PolarBelt.EAccelerometerRate.HZ50);

        //yield return new WaitForSeconds(0.5f);
        //_polarBelt.StartAccelerometer(PolarBelt.EAccelerometerRate.HZ50);
    }
}
