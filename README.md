# ble-unity-plugin-demo

This project has a minimum API requirement of Android 7.1.1. Use any Android smartphone with the minimum requirement to run.

To test, turn on the Arduino Nano 33 BLE while running @tmose1106's BLEButton script (https://github.com/Agora-VR/arduino-ble/tree/master/BLE_Button).

If you have Bluetooth or Location services disabled on your phone, "BLE Setup Success" will be False. Be sure to accept the location access request when loading the app up for the first time.

The plugin scans for nearby BLE devices for 10 seconds. After 10 seconds it will stop scanning (to save battery life). If the Arduino is on and running the BLE script, your device should be able to find it and output the "Scanned Device Info" data. There is a few seconds of delay to allow the plugin to find the device (doesn't actually take that long to find it, but I added the delay in case there is interference or some other factor that causes it to not find the device immediately.)

If you miss the 10 second window to either accept the location permission or have the Arduino running nearby, you can press the "Scan" button at the bottom of the screen to scan for another 10 seconds.


### Sample Logcat Output

You can also monitor the Logcat output to to see the debug information.

```
D/UnityBLEPlugin: Attempting to set up Bluetooth...
D/UnityBLEPlugin: Bluetooth set up Success!
D/UnityBLEPlugin: Scanning for BLE Devices...
D/UnityBLEPlugin: Callback Type: 1
D/UnityBLEPlugin: Result: ScanResult{device=EF:4A:84:C0:81:16, scanRecord=ScanRecord [mAdvertiseFlags=6, mServiceUuids=[8bff20de-32fb-4350-bddb-afe103ef9640], mServiceSolicitationUuids=[], mManufacturerSpecificData={}, mServiceData={}, mTxPowerLevel=-2147483648, mDeviceName=BLEButton, mTransportBlocks=[]], rssi=-59, timestampNanos=151078680278137, eventType=27, primaryPhy=1, secondaryPhy=0, advertisingSid=255, txPower=127, periodicAdvertisingInterval=0}
   . . .
D/UnityBLEPlugin: Callback Type: 1
D/UnityBLEPlugin: Result: ScanResult{device=EF:4A:84:C0:81:16, scanRecord=ScanRecord [mAdvertiseFlags=6, mServiceUuids=[8bff20de-32fb-4350-bddb-afe103ef9640], mServiceSolicitationUuids=[], mManufacturerSpecificData={}, mServiceData={}, mTxPowerLevel=-2147483648, mDeviceName=BLEButton, mTransportBlocks=[]], rssi=-59, timestampNanos=151088089096102, eventType=27, primaryPhy=1, secondaryPhy=0, advertisingSid=255, txPower=127, periodicAdvertisingInterval=0}
D/UnityBLEPlugin: Stopping Scan for BLE Devices...
```