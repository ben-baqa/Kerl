using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using TMPro;
using UnityEngine.UI;

public class ProfileMenu : MonoBehaviour
{
    public GameObject profileEntryPrefab;
    public Transform profileList;

    public GameObject splitView;
    public GameObject singleView;

    public Button newProfileButton, confirmButton, singleConfirmButton;
    public GameObject newProfileInput;

    TMP_InputField splitEditField, singleEditField;

    [HideInInspector]
    public string headsetID, profile;
    [HideInInspector]
    public TrainingMenu trainingMenu;

    void Awake()
    {
        splitEditField = newProfileInput.GetComponentInChildren<TMP_InputField>(true);
        singleEditField = singleView.GetComponentInChildren<TMP_InputField>(true);

        newProfileButton.onClick.AddListener(() =>
        {
            newProfileButton.gameObject.SetActive(false);
            newProfileInput.SetActive(true);
            splitEditField.Select();
        });

        confirmButton.onClick.AddListener(() => CreateNewProfile(splitEditField.text));
        singleConfirmButton.onClick.AddListener(() => CreateNewProfile(singleEditField.text));

        newProfileInput.SetActive(false);
        singleView.SetActive(false);
    }

    void OnEnable()
    {
        Cortex.profiles.ProfileQueryResult += OnProfileQueryResult;
        Cortex.profiles.QueryProfiles();
    }
    //void OnDisable() => Cortex.profiles.ProfileQueryResult -= OnProfileQueryResult;

    void OnProfileQueryResult(List<string> names)
    {
        foreach (Transform child in profileList)
            Destroy(child.gameObject);

        bool noExtantProfiles = names.Count == 0;

        singleView.SetActive(noExtantProfiles);
        splitView.SetActive(!noExtantProfiles);

        foreach (string profileName in names)
        {
            Instantiate(profileEntryPrefab, profileList).GetComponent<ProfileListEntry>().Init(profileName, headsetID, trainingMenu);
        }
    }

    public void CreateNewProfile(string profileName)
    {
        Cortex.profiles.CreateProfile(profileName);
        Cortex.profiles.ProfileCreated += LoadProfile;

        //trainingMenu.Init(true);
    }

    public void LoadProfile(string profileName)
    {
        Cortex.profiles.LoadProfile(profileName, headsetID);
        //trainingMenu.Init(true);

        profile = profileName;
    }

    public void UnloadProfile()
    {
        Cortex.profiles.UnloadProfile(profile, headsetID);
    }
}
