using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmotivUnityPlugin;

public class ConnectableHeadset : MonoBehaviour
{
    public TextMeshProUGUI deviceName;
    public Button bluetoothButton, dongleButton, cableButton, pairButton;
    public Transform connectingText;

    string headsetID;

    void Start()
    {
        bluetoothButton.onClick.AddListener(InitiateConnection);
        dongleButton.onClick.AddListener(InitiateConnection);
        cableButton.onClick.AddListener(InitiateConnection);
        pairButton.onClick.AddListener(Pair);
    }

    private void OnEnable()
    {
        Cortex.HeadsetConnected += OnHeadsetConnected;
    }

    private void OnDisable()
    {
        Cortex.HeadsetConnected -= OnHeadsetConnected;
    }

    public void Init(Headset info)
    {
        headsetID = info.headsetID;
        deviceName.text = headsetID;

        bool connected = info.status == "connected";

        pairButton.gameObject.SetActive(!connected);
        dongleButton.gameObject.SetActive(connected && info.connectedBy == ConnectionType.CONN_TYPE_DONGLE);
        bluetoothButton.gameObject.SetActive(connected && info.connectedBy == ConnectionType.CONN_TYPE_BTLE);
        cableButton.gameObject.SetActive(connected && info.connectedBy == ConnectionType.CONN_TYPE_USB_CABLE);
    }

    void Pair()
    {
        Cortex.ConnectDevice(headsetID);

        pairButton.gameObject.SetActive(false);
        connectingText.gameObject.SetActive(true);
    }

    void OnHeadsetConnected(HeadsetConnectEventArgs args)
    {
        if (args.HeadsetId == headsetID)
            InitiateConnection();
    }

    void InitiateConnection()
    {
        bluetoothButton.gameObject.SetActive(false);
        dongleButton.gameObject.SetActive(false);
        cableButton.gameObject.SetActive(false);

        connectingText.gameObject.SetActive(true);

        Cortex.StartSession(headsetID);
    }
}
