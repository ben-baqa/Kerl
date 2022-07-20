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
        Cortex.profiles.ProfileLoaded += OnProfileLoaded;
        Cortex.profiles.GuestProfileLoaded += OnGuestProfileLoaded;
    }
    void OnDisable()
    {
        Cortex.ConnectionStateChanged -= OnConnectionStateChanged;
        Cortex.DataStreamStarted -= OnDataStreamStarted;
        Cortex.profiles.ProfileLoaded -= OnProfileLoaded;
        Cortex.profiles.GuestProfileLoaded -= OnGuestProfileLoaded;
    }

    public void Continue()
    {
        state += 1;
        if (state > SetupMenuState.TRAINING)
            Return();
        ApplyState();
    }

    public void Back()
    {
        switch (state)
        {
            case SetupMenuState.CONTACT_QUALITY:
                // unpair headset
                Cortex.EndMostRecentSession();
                break;
            case SetupMenuState.TRAINING:
                // unload profile
                profileMenu.UnloadProfile();

                trainingMenu.ResetMenu();
                break;
        }
        state -= 1;
        ApplyState();
    }

    public void Return()
    {
        // trigger return to whatever menu you came from
        returnMenu.SetActive(true);
        canvas.enabled = false;
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

    void OnGuestProfileLoaded(string profileName)
    {
        Continue();
        trainingMenu.GuestInit();
    }

    public void ActivateFrom(GameObject menu)
    {
        canvas.enabled = true;
        returnMenu = menu;
        returnMenu.SetActive(false);

        state = SetupMenuState.CONNECTING;
        ApplyState();
        if (connectedToCortex)
            Continue();

        trainingMenu.ResetMenu();
    }
}

public interface IRequiresInit
{
    public void Init();
}