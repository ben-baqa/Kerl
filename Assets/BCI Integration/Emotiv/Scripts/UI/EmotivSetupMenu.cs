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
        Cortex.HeadsetConnected += OnHeadsetConnected;
        Cortex.training.ProfileLoaded += OnProfileLoaded;
    }
    void OnDisable()
    {
        Cortex.ConnectionStateChanged -= OnConnectionStateChanged;
        Cortex.HeadsetConnected -= OnHeadsetConnected;
    }

    public void Continue()
    {
        state += 1;
        if(state > SetupMenuState.TRAINING)
        {
            returnMenu.SetActive(true);
            //baseObject.SetActive(false);
            canvas.enabled = false;
            // probably trigger return to whatever menu you came from
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
            Continue();
        connectionStateDisplay.OnConnectionStateChanged(connectionState);
    }

    void OnHeadsetConnected(string headsetID)
    {
        contactQualityDisplay.Activate(headsetID);
        profileMenu.headsetID = headsetID;
        //trainingMenu.OnHeadsetConnected(headsetID);
        Continue();
    }

    void OnProfileLoaded(string profileName)
    {
        print("profile loaded, getting trained actions");
        Cortex.profiles.GetTrainedActions(profileName);
        Continue();
    }

    public void ActivateFrom(GameObject menu)
    {
        canvas.enabled = true;
        //baseObject.SetActive(true);
        returnMenu = menu;
        returnMenu.SetActive(false);
    }
}

public interface IRequiresInit
{
    public void Init();
}