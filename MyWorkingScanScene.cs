using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class MyWorkingScanScene : MonoBehaviour
{

    // Use this for initialization
    BluetoothHelper bluetoothHelper;
    string deviceName;
    BluetoothHelperCharacteristic arduinoCharacteristic;


    void Start()
    {
        Debug.Log("Starting BLE Script");
        
        try
        {
            // Debug.Log(getNumber());  Debug.Log(Application.unityVersion);
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance("Arduino");
            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.OnScanEnded += OnScanEnded;

            bluetoothHelper.setTerminatorBasedStream("\n");
            //bluetoothHelper.setLengthBasedStream();
            //bluetoothHelper.setFixedLengthBasedStream(10);

            // if(bluetoothHelper.isDevicePaired())
            // 	sphere.GetComponent<Renderer>().material.color = Color.blue;
            // else
            // 	sphere.GetComponent<Renderer>().material.color = Color.grey;
            // bluetoothHelper.ScanNearbyDevices();

            bluetoothHelper.ScanNearbyDevices();
        }
        catch (BluetoothHelper.BluetoothHelperException ex)
        {

            Debug.Log(ex.ToString());

        }
    }



    //Asynchronous method to receive messages
    void OnMessageReceived()
    {
        //StartCoroutine(blinkSphere());
        string received_message = bluetoothHelper.Read();
        //text.text = received_message;
        Debug.Log(System.DateTime.Now.Second);
        Debug.Log(received_message);
    }

    void OnScanEnded(LinkedList<BluetoothDevice> nearbyDevices)
    {
        Debug.Log("Found " + nearbyDevices.Count + " devices");

        foreach (var x in nearbyDevices)
        {
            Debug.Log(x.DeviceName);
            if (x.DeviceName.StartsWith("Arduino"))
            {
                Debug.Log("Setting Name...");
                bluetoothHelper.setDeviceAddress(x.DeviceAddress);
                break;
            }

        }

        // bluetoothHelper.setDeviceName("Arduino");
        if (!bluetoothHelper.isDevicePaired())
        {
            bluetoothHelper.ScanNearbyDevices();
            return;
        }


        // bluetoothHelper.setDeviceAddress("00:21:13:02:16:B1");
        bluetoothHelper.Connect();
        //bluetoothHelper.isDevicePaired();
    }

    public void TriggerHaptics()
    {
        byte[] duration = new byte[1];
        duration[0] = 2;
        if (arduinoCharacteristic != null)
            bluetoothHelper.WriteCharacteristic(arduinoCharacteristic, duration);
    }


    void Update()
    {
        //Debug.Log("Sending message...");
        //Debug.Log(bluetoothHelper.IsBluetoothEnabled());
        //TriggerHaptics();
    }

    void OnConnected()
    {
        foreach (var x in bluetoothHelper.getGattServices())
        {
            Debug.Log(x.getName());
            foreach (var y in x.getCharacteristics())
            {
                Debug.Log(">" + y.getName());
                if (y.getName().Equals("19b10001-E8F2-537E-4F6C-D104768A1214"))
                {
                    Debug.Log("Located characteristic, saving reference");
                    arduinoCharacteristic = y;
                }
                arduinoCharacteristic = y;
            }
        }
        try
        {
            bluetoothHelper.StartListening();
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
        }

        Debug.Log("All characteristics listed");
    }

    void OnConnectionFailed()
    {
        // sphere.GetComponent<Renderer>().material.color = Color.red;
        Debug.Log("Connection Failed");
    }


    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }
}
