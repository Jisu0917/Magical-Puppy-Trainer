using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;

public class PluginWrapper2 : MonoBehaviour
{
    // For Inspector View Only
    [Header("Plugin & Device Names")]
    [ReadOnly] public string _pluginName = "com.magical.puppy.trainer.androidBLE";
    private const string pluginName = "com.magical.puppy.trainer.androidBLE";

    [SerializeField] private string BLEDeviceName = "MyPet-BLE"; //TODO: Placeholder device name
    [SerializeField] private string serviceUUID = "8bff20de-32fb-4350-bddb-afe103ef9640";
    [SerializeField] private string heartRateUUID = "1c8dd778-e8c3-45b0-a9f3-48c33a400315";
    [SerializeField] private string pulseOximetryUUID = "b8ae0c39-6204-407c-aa43-43087ec29a63";

    [Header("Read Data Interval")]
    [SerializeField] private float delay = 1000;

    private bool stop = false;
    private static AndroidJavaClass _pluginClass;
    private static AndroidJavaObject _pluginInstance;

    public static string shape = "";

    public static AndroidJavaClass PluginClass
    {
        get
        {
            if (_pluginClass == null)
            {
                _pluginClass = new AndroidJavaClass(pluginName);
            }
            return _pluginClass;
        }
    }

    public static AndroidJavaObject PluginInstance
    {
        get
        {
            if (_pluginInstance == null)
            {
                _pluginInstance = PluginClass.CallStatic<AndroidJavaObject>("getInstance");
            }
            return _pluginInstance;
        }
    }

    void Start()
    {
        checkPermissions(); // Make sure we have the correct permissions for BLE

        if (Input.location.isEnabledByUser)
        {
            StartCoroutine(startBLE());         // Starts setup immediately if permissions are already enabled.
        }
        else
        {
            StartCoroutine(waitForLocation());  // For first time opening of app.
        }
    }

    private IEnumerator startBLE()
    {
        // Get Android activity for context.
        AndroidJavaClass playerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivityObject = playerClass.GetStatic<AndroidJavaObject>("currentActivity");

        // Setup BLE using plugin calls.
        PluginInstance.Call("setContext", currentActivityObject);
        
        // Connect to device after a few seconds of scan time.
        yield return new WaitForSeconds(5);
        PluginInstance.Call("connectToDevice", serviceUUID, heartRateUUID, pulseOximetryUUID);
        StartCoroutine(discover());
    }

    // Gets data from plugin at intervals of 'delay' seconds.
    private IEnumerator getData()
    {
        Debug.Log(PluginInstance.Get<int>("heartRate"));
        Debug.Log(PluginInstance.Get<int>("pulseOximetry"));

        int accuracy = PluginInstance.Get<int>("pulseOximetry");

        if (accuracy > 50)
        {
            int shapeInt = PluginInstance.Get<int>("heartRate");
            switch (shapeInt)
            {
                case 0:
                    // circle
                    shape = "circle";
                    break;
                case 1:
                    // heart
                    shape = "heart";
                    break;
                case 2:
                    // star
                    shape = "star";
                    break;
            }

            MakeHeart2.shape = shape;
            MakeHeart2.acc = accuracy;
        }
        else
        {
            shape = "none";
        }

        yield return new WaitForSeconds(delay);
        StartCoroutine(getData());
    }

    // Show on UI if services are discovered (takes several seconds)
    private IEnumerator discover()
    {
        if (stop)
        {
            // do nothing
        }
        else if (PluginInstance.Get<bool>("discovered"))
        {
            StartCoroutine(getData());
        }
        else
        {
            yield return new WaitForSeconds(1);
            StartCoroutine(discover());
        }
    }

    // Delays for 10 seconds for the user to accept location permission.
    private IEnumerator waitForLocation()
    {
        yield return new WaitForSeconds(10);
        StartCoroutine(startBLE());
    }

    private void checkPermissions()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
    }

    public void scanButton()
    {
        reset();
        stop = false;
        StartCoroutine(startBLE());
    }

    public void reset()
    {
        stop = true;
        StopCoroutine(discover());
        StopCoroutine(getData());
        PluginInstance.Call("close");
    }

    void OnApplicationQuit()
    {
        PluginInstance.Call("close");
    }

}
