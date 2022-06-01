using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

/* handles the flow between different training submenus including those for returning players
 * new players, active training, and validation of extant training
 */
public class TrainingMenu : MonoBehaviour, IRequiresInit
{
    public int minTrainingRounds = 16, trainingCountdownTime = 4;
    public GameObject returningView;
    public GameObject trainingExplanation;
    public GameObject validationView;

    public Transform feedback, trainingLocation, validationLocation;

    Animator feedbackAnim;
    Vector3 trainPos, valPos, trainRot, valRot;
    TrainingSubmenu training;


    string headsetID;
    bool validating = false;

    public void Init()
    {
        Cortex.HeadsetConnected += OnHeadsetConnected;
        Cortex.training.GetTrainedActionsResult += Init;
        //Cortex.training.ProfileLoaded += (string s) => profileName = s;
        feedbackAnim = feedback.GetComponentInChildren<Animator>(true);
        trainPos = trainingLocation.localPosition;
        trainRot = trainingLocation.localEulerAngles;
        valPos = validationLocation.localPosition;
        valRot = validationLocation.localEulerAngles;

        returningView.SetActive(false);
        trainingExplanation.SetActive(false);
        validationView.SetActive(false);
        feedback.gameObject.SetActive(false);

        training = GetComponentInChildren<TrainingSubmenu>(true);
        training.Init();
        training.OnTrainingComplete = OnTrainingSequenceComplete;
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
        print($"training menu intiated! {trainedActions.totalTimesTraining}");

        if (trainedActions.totalTimesTraining < minTrainingRounds)
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
    }

    public void OnHeadsetConnected(string id)
    {
        print("Headset ID received successfully!");
        print($"training: {training}");
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
        // add new bci input to input handler
        InputProxy.AddInput(new BCIInput(new EmotivHeadsetProxy(headsetID)));
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
        validationView.SetActive(true);

        feedback.gameObject.SetActive(true);
        feedbackAnim.SetBool("brushing", true);
    }

    void OnTrainingSequenceComplete()
    {
        validating = true;
        training.gameObject.SetActive(false);
        validationView.SetActive(true);
    }
}