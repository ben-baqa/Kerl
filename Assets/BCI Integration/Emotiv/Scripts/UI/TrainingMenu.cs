using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

/* handles the flow between different training submenus including those for returning players
 * new players, active training, and validation of extant training
 */
public class TrainingMenu : MonoBehaviour, IRequiresInit
{
    [Header("settings")]
    public int minTrainingRounds = 16, trainingCountdownTime = 4;
    [Header("References")]
    public GameObject returningView;
    public GameObject trainingExplanation;
    public GameObject validationView;

    public Transform feedback;
    public Transform trainingLocation;
    public Transform validationLocation;
    [Header("ice settings")]
    public MeshRenderer iceMesh;
    public float iceScrollSpeed;

    Animator feedbackAnim;
    Vector3 trainPos, valPos, trainRot, valRot;
    TrainingSubmenu training;

    Material iceMat;
    Vector3 iceOffset = Vector3.zero;

    string headsetID, profileName;
    bool validating = false;
    bool saveProfile = true;

    public void Init()
    {
        Cortex.DataStreamStarted += OnDataStreamStarted;
        Cortex.training.GetTrainedActionsResult += Init;
        Cortex.training.ProfileLoaded += (string s) => profileName = s;

        feedbackAnim = feedback.GetComponentInChildren<Animator>(true);
        trainPos = trainingLocation.localPosition;
        trainRot = trainingLocation.localEulerAngles;
        valPos = validationLocation.localPosition;
        valRot = validationLocation.localEulerAngles;

        iceMat = iceMesh.material;

        returningView.SetActive(false);
        trainingExplanation.SetActive(false);
        validationView.SetActive(false);
        feedback.gameObject.SetActive(false);

        training = GetComponentInChildren<TrainingSubmenu>(true);
        training.Init();
        training.OnTrainingComplete = OnTrainingSequenceComplete;
    }

    public void ResetMenu()
    {
        returningView.SetActive(false);
        trainingExplanation.SetActive(false);
        validationView.SetActive(false);
        feedback.gameObject.SetActive(false);

        training.Init();
    }

    public void OnEnable()
    {
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
    }
    public void OnDisable()
    {
        Cortex.UnsubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        feedback.gameObject.SetActive(false);
    }

    public void Init(TrainedActions trainedActions)
    {
        //print($"training menu intiated! {trainedActions.totalTimesTraining}");

        if (trainedActions.trainingCount < minTrainingRounds * 2)
        {
            trainingExplanation.SetActive(true);
        }
        else
        {
            returningView.SetActive(true);
        }
        feedback.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (validating)
        {
            feedback.localPosition = Vector3.Lerp(feedback.localPosition, valPos, 0.1f);
            feedback.localEulerAngles = Vector3.Lerp(feedback.localEulerAngles, valRot, 0.1f);
        }
        else
        {
            feedback.localPosition = Vector3.Lerp(feedback.localPosition, trainPos, 0.1f);
            feedback.localEulerAngles = Vector3.Lerp(feedback.localEulerAngles, trainRot, 0.1f);
        }

        if (feedbackAnim.GetCurrentAnimatorStateInfo(0).IsName("brush"))
            iceOffset += Vector3.right * iceScrollSpeed * feedbackAnim.GetFloat("brush speed");
        iceMat.SetVector("_offset", iceOffset);
    }

    public void OnDataStreamStarted(string id)
    {
        headsetID = id;
        training.headsetID = id;
    }

    void OnMentalCommandRecieved(MentalCommand command)
    {
        if (validating && command.action != "neutral")
            feedbackAnim.SetFloat("brush speed", (float)command.power);
    }

    public void FinishTraining()
    {
        // save profile if training was not skipped by debug
        if (saveProfile)
            Cortex.training.SaveProfile(profileName, headsetID);
        // add new bci input to input handler
        InputProxy.AddInput(new BCIInput(new EmotivHeadsetProxy(headsetID, profileName)));
    }

    public void ContinueTraining()
    {
        // return to training with button to switch to validation view
        validating = false;
        training.ResetTraining(4);
        validationView.SetActive(false);
    }

    public void ResetTraining()
    {
        validating = false;

        Cortex.training.EraseTraining("push");
        Cortex.training.EraseTraining("neutral");

        training.ResetTraining();
        validationView.SetActive(false);
    }

    public void SkipToValidation()
    {
        validating = true;
        returningView.SetActive(false);
        trainingExplanation.SetActive(false);
        training.gameObject.SetActive(false);
        validationView.SetActive(true);

        feedback.gameObject.SetActive(true);
        feedbackAnim.SetBool("brushing", true);
    }
    public void DebugSkipToValidation()
    {
        saveProfile = false;
        SkipToValidation();
    }

    void OnTrainingSequenceComplete()
    {
        validating = true;
        training.gameObject.SetActive(false);
        validationView.SetActive(true);
    }
}