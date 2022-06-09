using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class EmotivSetupMenu : MonoBehaviour
{
    // TODO: add menu stage for breif description of how to put the headset on
    public enum SetupMenuState
    {
        CONNECTING,
        PAIRING_HEADSET,
        //FITTING_HEADSET,
        CONTACT_QUALITY,
        PROFILE_SELECTION,
        TRAINING
    }

    SetupMenuState state = SetupMenuState.CONNECTING;

    ConnectionStateDisplay connectionStateDisplay;
    HeadsetPairingMenu headsetPairingMenu;
    ContactQualityDisplay contactQualityDisplay;
    ProfileMenu profileMenu;
    TrainingMenu trainingMenu;

    public GameObject returnMenu, baseObject;
    Canvas canvas;
    bool connectedToCortex = false;
    
    void Start()
    {
        connectionStateDisplay = GetComponentInChildren<ConnectionStateDisplay>(true);
        headsetPairingMenu = GetComponentInChildren<HeadsetPairingMenu>(true);
        contactQualityDisplay = GetComponentInChildren<ContactQualityDisplay>(true);
        profileMenu = GetComponentInChildren<ProfileMenu>(true);
        trainingMenu = GetComponentInChildren<TrainingMenu>(true);
        trainingMenu.Init();

        canvas = GetComponent<Canvas>();

        profileMenu.trainingMenu = trainingMenu;

        ApplyState();
        //baseObject.SetActive(false);
        canvas.enabled = false;
    }
    void OnEnable()
    {
        Cortex.ConnectionStateChanged += OnConnectionStateChanged;
        Cortex.DataStreamStarted += OnDataStreamStarted;
        Cortex.training.ProfileLoaded += OnProfileLoaded;
    }
    void OnDisable()
    {
        Cortex.ConnectionStateChanged -= OnConnectionStateChanged;
        Cortex.DataStreamStarted -= OnDataStreamStarted;
    }

    public void Continue()
    {
        state += 1;
        if(state > SetupMenuState.TRAINING)
        {
            // trigger return to whatever menu you came from
            returnMenu.SetActive(true);
            canvas.enabled = false;
        }
        ApplyState();
    }

    void ApplyState()
    {
        connectionStateDisplay.gameObject.SetActive(state == SetupMenuState.CONNECTING);
        headsetPairingMenu.gameObject.SetActive(state == SetupMenuState.PAIRING_HEADSET);
        contactQualityDisplay.gameObject.SetActive(state == SetupMenuState.CONTACT_QUALITY);
        profileMenu.gameObject.SetActive(state == SetupMenuState.PROFILE_SELECTION);
        trainingMenu.gameObject.SetActive(state == SetupMenuState.TRAINING);
    }

    void OnConnectionStateChanged(ConnectToCortexStates connectionState)
    {
        if (connectionState == ConnectToCortexStates.Authorized)
        {
            Continue();
            connectedToCortex = true;
        }
        connectionStateDisplay.OnConnectionStateChanged(connectionState);
    }

    void OnDataStreamStarted(string headsetID)
    {
        profileMenu.headsetID = headsetID;
        contactQualityDisplay.Activate(headsetID);
        Continue();
    }

    void OnProfileLoaded(string profileName)
    {
        Cortex.profiles.GetTrainedActions(profileName);
        Continue();
    }

    public void ActivateFrom(GameObject menu)
    {
        canvas.enabled = true;
        returnMenu = menu;
        returnMenu.SetActive(false);

        state = SetupMenuState.CONNECTING;
        if (connectedToCortex)
            Continue();
        else
            ApplyState();

        trainingMenu.ResetMenu();
    }
}

public interface IRequiresInit
{
    public void Init();
}