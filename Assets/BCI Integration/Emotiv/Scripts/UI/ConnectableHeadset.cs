using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EmotivUnityPlugin;

public class ConnectableHeadset : MonoBehaviour
{
    public TextMeshProUGUI deviceName;
    public Button bluetoothButton, dongleButton, cableButton;
    public Transform connectingText;

    string headsetID;
    Headset headsetInfo;

    void Start()
    {
        bluetoothButton.onClick.AddListener(InitiateConnection);
        dongleButton.onClick.AddListener(InitiateConnection);
        cableButton.onClick.AddListener(InitiateConnection);

        DataProcessing.Instance.HeadsetConnected += OnConnection;
        DataProcessing.Instance.HeadsetConnectFail += OnConnectionFailed;
    }
    private void OnDestroy()
    {
        DataProcessing.Instance.HeadsetConnected -= OnConnection;
        DataProcessing.Instance.HeadsetConnectFail -= OnConnectionFailed;
    }

    public void Init(Headset info)
    {
        headsetInfo = info;
        headsetID = info.HeadsetID;
        deviceName.text = headsetID;

        dongleButton.gameObject.SetActive(info.HeadsetConnection == ConnectionType.CONN_TYPE_DONGLE);
        bluetoothButton.gameObject.SetActive(info.HeadsetConnection == ConnectionType.CONN_TYPE_BTLE);
        cableButton.gameObject.SetActive(info.HeadsetConnection == ConnectionType.CONN_TYPE_USB_CABLE);
    }

    void InitiateConnection()
    {
        bluetoothButton.gameObject.SetActive(false);
        dongleButton.gameObject.SetActive(false);
        cableButton.gameObject.SetActive(false);

        connectingText.gameObject.SetActive(true);

        List<string> dataStreamList = new List<string>() {
            DataStreamName.SysEvents, DataStreamName.MentalCommands, DataStreamName.DevInfos };
        DataStreamManager.Instance.StartDataStream(dataStreamList, headsetID);
        DataProcessing.Instance.SetConnectedHeadset(headsetInfo);
    }

    void OnConnection(object sender, string headsetId)
    {
        print("====== HEADSET CONNECTION SUCCESS");
        connectingText.gameObject.SetActive(false);
    }

    void OnConnectionFailed(object sender, string headsetId)
    {
        print("====== HEADSET CONNECTION FAILURE");
    }
}
