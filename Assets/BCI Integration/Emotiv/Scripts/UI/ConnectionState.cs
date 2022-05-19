using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using TMPro;

/// <summary>
/// Keeps track of connection state to update UI
/// </summary>
public class ConnectionState : MonoBehaviour
{
    ConnectToCortexStates connectionState = ConnectToCortexStates.Service_connecting;
    ConnectToCortexStates previousState;
    //bool connected = false;

    public TextMeshProUGUI stateText;
    public GameObject installApp, Login, headsetPairing;

    private void Start()
    {
        Authorizer.Instance.ConnectServiceStateChanged += OnConnectionStateChanged;
        ActOnStateChange();
    }
    private void OnDestroy()
    {
        Authorizer.Instance.ConnectServiceStateChanged -= OnConnectionStateChanged;
    }

    void Update()
    {
        if (connectionState != previousState)
        {
            previousState = connectionState;
            // Update UI elements
            ActOnStateChange();
        }
    }

    private void ActOnStateChange()
    {
        installApp.SetActive(connectionState == ConnectToCortexStates.EmotivApp_NotFound);
        Login.SetActive(connectionState == ConnectToCortexStates.Login_notYet);

        switch (connectionState)
        {
            case ConnectToCortexStates.Service_connecting:
                {
                    stateText.text = "Connecting to Service...";
                    Debug.Log("=============== Connecting To service");
                    break;
                }
            case ConnectToCortexStates.EmotivApp_NotFound:
                {
                    stateText.text = "";
                    Debug.Log("=============== Connect_failed");
                    break;
                }
            case ConnectToCortexStates.Login_waiting:
                {
                    stateText.text = "Waiting for login";
                    Debug.Log("=============== Login_waiting");
                    break;
                }
            case ConnectToCortexStates.Login_notYet:
                {
                    stateText.text = "";
                    Debug.Log("=============== Login_notYet");
                    break;
                }
            case ConnectToCortexStates.Authorizing:
                {
                    stateText.text = "Authenticating...";
                    Debug.Log("=============== Authorizing");
                    break;
                }
            case ConnectToCortexStates.Authorize_failed:
                {
                    stateText.text = "Authentication Failed";
                    Debug.Log("=============== Authorize_failed");
                    break;
                }
            case ConnectToCortexStates.Authorized:
                {
                    headsetPairing.SetActive(true);
                    gameObject.SetActive(false);
                    Debug.Log("=============== Authorized");
                    break;
                }
            case ConnectToCortexStates.LicenseExpried:
                {
                    stateText.text = "Trial Expired";
                    Debug.Log("=============== Trial expired");
                    break;
                }
            case ConnectToCortexStates.License_HardLimited:
                {
                    stateText.text = "Offline use limit reached";
                    Debug.Log("=============== License_HardLimited");
                    break;
                }
        }
    }

    private void OnConnectionStateChanged(object sender, ConnectToCortexStates newState)
    {
        connectionState = newState;
    }

    public void InstallApp()
    {
        Application.OpenURL("https://www.emotiv.com/emotiv-launcher/#download");
    }
}
