using PlayBionic.MyoArmbandAPI.Armband;
using PlayBionic.MyoArmbandAPI.Manager;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MyoArmbandProvider : MonoBehaviour {

    /// <summary>
    /// Receive EMG samples as 8x signed byte values (-128 to 127)
    /// </summary>
    public event Action<sbyte[]> OnEMGSampleReceived;

    public MyoArmband MyoArmband { get; private set; }

    [SerializeField] private string deviceAddress = "";

    private float _connectCooldown = 2f;
    private List<EmgDataSample> _receivedSamples = new();

    private void Awake() {
        Application.wantsToQuit += Application_wantsToQuit;

        // Initialize the API
        if (!MyoArmbandManager.isInitialized()) {
#if UNITY_EDITOR
            try {
                MyoArmbandManager.initialize(new PlayBionic.MyoArmbandAPI.Windows.WindowsAdapter("pb_myo_bridge"));
            } catch (Exception) {
                Debug.LogWarning("Cannot initialize MyoArmband API");
            }
#elif UNITY_ANDROID && !UNITY_EDITOR
        MyoArmbandManager.initialize(new PlayBionic.MyoArmbandAPI.Android.AndroidAdapter());
#endif
        }

        // Create Device
        MyoArmband = MyoArmbandManager.createArmband("Myo", deviceAddress);
        MyoArmband.OnConnectionStateChanged += MyoArmband_OnConnectionStateChanged;
        MyoArmband.OnBatteryLevelUpdated += MyoArmband_OnBatteryLevelUpdated;
        MyoArmband.OnEmgDataReceived += MyoArmband_OnEmgDataReceived;
    }

    private bool Application_wantsToQuit() {
        // Disconnect the device
        if (MyoArmband != null) {
            if (MyoArmband.ConnectionState == EConnectionState.Connected || MyoArmband.ConnectionState == EConnectionState.Connecting) {
                // Set sleep mode back to normal to save battery!
                MyoArmband.SetSleepMode(ESleepMode.Normal);
                MyoArmband.Disconnect();
            }
        }

        // Shut down the API to free the actual bluetooth resources
        MyoArmbandManager.shutDown();
        return true;
    }

    private void Update() {
        if (!MyoArmbandManager.isInitialized() || MyoArmband == null) { return; }

        // Keep the armbands connected        
        if (_connectCooldown == 0) {
            if (MyoArmband.ConnectionState == EConnectionState.Disconnected ||
                MyoArmband.ConnectionState == EConnectionState.Failed) {
                Debug.Log($"Connect Myo...");
                MyoArmband.Connect();
            }
        } else if (_connectCooldown > 0) {
            _connectCooldown -= Time.deltaTime;
            if (_connectCooldown < 0) {
                _connectCooldown = 0;
            }
        }

        if (MyoArmband.ConnectionState == EConnectionState.Connected) {
            // The current orientation of the Armband. We use it to set the rotation of the transform this component is attached to
            transform.rotation = MyoArmband.ImuData.Orientation;

            //Copy received samples from threadsafe object
            List<EmgDataSample> samples;
            lock (_receivedSamples) {
                samples = new(_receivedSamples);
                _receivedSamples.Clear();
            }

            //Process samples
            foreach (EmgDataSample s in samples) {
                OnEMGSampleReceived?.Invoke(s.get());
            }
        }
    }

    private void MyoArmband_OnConnectionStateChanged(EConnectionState connectionState) {
        Debug.Log("Myo connection state changed to " + connectionState);
        if (connectionState == EConnectionState.Connected) {
            // Set sleep mode to never to prevent the device from turning off when it is not moved
            MyoArmband.SetSleepMode(ESleepMode.NeverSleep);

            // Start submission of orientation data
            MyoArmband.ImuMode = EImuMode.Filtered;

            // Start submission of EMG data
            MyoArmband.EmgMode = EEmgMode.Filtered;
        }
    }

    private void MyoArmband_OnBatteryLevelUpdated(int batteryLevel) {
        Debug.Log($"Myo Battery: {batteryLevel}%");
    }

    private void MyoArmband_OnEmgDataReceived(EmgDataSample[] samples) {
        lock (_receivedSamples) {
            _receivedSamples.Add(samples[0]);
        }
    }
}
