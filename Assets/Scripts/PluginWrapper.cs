using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PluginWrapper : MonoBehaviour
{
    // For Inspector View Only
    [Header("Plugin & Device Names")]
    [ReadOnly] public string _pluginName = "com.rutgersece.capstone2020.agoravr.blelibrary.AndroidBLE";
    private const string pluginName = "com.rutgersece.capstone2020.agoravr.blelibrary.AndroidBLE";

    [SerializeField] private string BLE_DeviceName = "BLEButton"; //TODO: Placeholder device name

    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;

    public static AndroidJavaClass PluginClass
    {
        get {
            if(_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(pluginName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get {
            if(_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }
    }

    [Header("Textboxes")]
    [SerializeField] private TextMeshProUGUI BLESetupSuccess;
    [SerializeField] private TextMeshProUGUI BLETarget;
    [SerializeField] private TextMeshProUGUI BLEInfo;

    [Header("Other Fields")]
    [SerializeField] private int delay = 5;

    void Start()
    {
        checkPermissions(); // Make sure we have the correct permissions for BLE

        if(Input.location.isEnabledByUser) {
            BLEsetup();                         // Starts setup immediately if permissions are already enabled.
        } else {
            StartCoroutine(waitForLocation());  // Delays for 10 seconds for the user to accept location permission.
        }
    }

    private void BLEsetup()
    {
        // Get Android Activity For Context
        AndroidJavaClass playerClass = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivityObject = playerClass.GetStatic<AndroidJavaObject> ("currentActivity");

        // Setup BLE Using Plugin Calls
        PluginInstance.Call("setContext", currentActivityObject);
        BLESetupSuccess.text = "BLE Setup Success: " + PluginInstance.Call<bool>("BLEsetup", BLE_DeviceName).ToString();
        BLETarget.text = "Target Device Name: " + BLE_DeviceName;

        StartCoroutine(getBLEInfo());
    }

    private void checkPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation)) {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    public void scanButton()
    {
        PluginInstance.Call<bool>("BLEsetup", BLE_DeviceName);
    }

    IEnumerator getBLEInfo()
    {
        yield return new WaitForSeconds(delay);
        BLEInfo.text = "Scanned Device Info: " + PluginInstance.Get<string>("btDeviceInfo");
        StartCoroutine(getBLEInfo());
    }

    IEnumerator waitForLocation()
    {
        yield return new WaitForSeconds(10);
        BLEsetup();
    }

}
