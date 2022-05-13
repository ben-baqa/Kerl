using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

/// <summary>
/// Keeps track of connection state to update UI
/// </summary>
public class ConnectionState : MonoBehaviour
{
    DataStreamManager dataStream = DataStreamManager.Instance;

    ConnectToCortexStates connectionState;
    bool connected = false;

    float stateUpdateTimer = 0;
    const float STATE_UPDATE_INTERVAL = 0.5f;

    void Update()
    {
        if (connected)
            return;

        stateUpdateTimer += Time.deltaTime;
        if (stateUpdateTimer < STATE_UPDATE_INTERVAL)
            return;
        stateUpdateTimer -= STATE_UPDATE_INTERVAL;

        var newState = dataStream.GetConnectToCortexState();
        if (newState == connectionState)
            return;
        connectionState = newState;

        switch (connectionState)
        {
            case ConnectToCortexStates.Service_connecting:
            {
                Debug.Log("=============== Connecting To service");
                break;
            }
            case ConnectToCortexStates.EmotivApp_NotFound:
            {

                connected = true;
                //_installEmotivApp.Activate();
                //this.Deactivate();
                Debug.Log("=============== Connect_failed");
                break;
            }
            case ConnectToCortexStates.Login_waiting:
            {
                Debug.Log("=============== Login_waiting");
                break;
            }
            case ConnectToCortexStates.Login_notYet:
            {
                connected = true;
                //_loginViaEmotivApp.Activate();
                //this.Deactivate();
                Debug.Log("=============== Login_notYet");
                break;
            }
            case ConnectToCortexStates.Authorizing:
            {
                //_stateText.text = "Authenticating...";
                Debug.Log("=============== Authorizing");
                break;
            }
            case ConnectToCortexStates.Authorize_failed:
            {
                Debug.Log("=============== Authorize_failed");
                break;
            }
            case ConnectToCortexStates.Authorized:
            {
                connected = true;
                //ActivateHeadsetQuery();
                Debug.Log("=============== Authorized");
                break;
            }
            case ConnectToCortexStates.LicenseExpried:
            {
                connected = true;
                //_trialExpried.Activate();
                //this.Deactivate();
                Debug.Log("=============== Trial expired");
                break;
            }
            case ConnectToCortexStates.License_HardLimited:
            {
                connected = true;
                //_offlineUseLimit.Activate();
                //this.Deactivate();
                Debug.Log("=============== License_HardLimited");
                break;
            }
        }
    }
}
