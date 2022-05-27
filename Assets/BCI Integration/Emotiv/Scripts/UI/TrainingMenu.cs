using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using Newtonsoft.Json.Linq;
using TMPro;

public class TrainingMenu : MonoBehaviour
{
    enum TrainingState { NEUTRAL, BRUSHING, VALIDATION }
    TrainingState trainingState = TrainingState.NEUTRAL;

    string trainingAction => trainingState == TrainingState.BRUSHING ? "brush" : "neutral";

    public int minTrainingRounds = 16, trainingCountdownTime = 4;
    public GameObject returning, training, trainingExplanation, validation, trainingCompletionOptions;

    public Transform feedBackCharacter, trainingBrusherPosition, validationBrusherPosition;

    public TextMeshProUGUI trainingQualityText, countDownText;

    Animator feedbackAnim;
    Vector3 trainingPos, validationPos, trainingRot, validationRot;


    string headsetID;
    float trainingCountdownTimer = 0;
    int trainingCount = 0;
    bool realtimeFeedback = false, trainingCountdown = false;

    private void Start()
    {
        Cortex.HeadsetConnected += (string s) => headsetID = s;
        //Cortex.training.ProfileLoaded += (string s) => profileName = s;
        feedbackAnim = feedBackCharacter.GetComponentInChildren<Animator>(true);
        trainingPos = trainingBrusherPosition.localPosition;
        trainingRot = trainingBrusherPosition.localEulerAngles;
        validationPos = validationBrusherPosition.localPosition;
        validationRot = validationBrusherPosition.localEulerAngles;

        returning.SetActive(false);
        training.SetActive(false);
        trainingExplanation.SetActive(false);
        validation.SetActive(false);
        trainingCompletionOptions.SetActive(false);
        feedBackCharacter.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        Cortex.training.TrainingCompleted += OnTrainingComplete;
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        print("================ YEET");
    }
    public void OnDisable()
    {
        Cortex.training.TrainingCompleted -= OnTrainingComplete;
        Cortex.UnsubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
    }

    public void Init(bool newProfile = false)
    {
        if (newProfile)
        {
            trainingExplanation.SetActive(true);
            feedBackCharacter.gameObject.SetActive(true);
        }
        else
        {
            returning.SetActive(true);
        }
    }

    private void Update()
    {
        if (trainingState == TrainingState.VALIDATION)
        {
            feedBackCharacter.localPosition = Vector3.Lerp(feedBackCharacter.localPosition, validationPos, 0.1f);
            feedBackCharacter.localEulerAngles = Vector3.Lerp(feedBackCharacter.localEulerAngles, validationRot, 0.1f);
        }
        else
        {
            trainingCountdownTimer -= Time.deltaTime;
            countDownText.text = $"{(int)trainingCountdownTimer}";
            if (trainingCountdownTimer < 0)
            {
                if (trainingCountdown)
                {
                    Cortex.training.StartTraining(trainingAction);
                    trainingCountdownTimer = 8;
                    countDownText.enabled = false;
                }
                else
                    trainingCountdown = false;
            }

            feedBackCharacter.localPosition = Vector3.Lerp(feedBackCharacter.localPosition, trainingPos, 0.1f);
            feedBackCharacter.localEulerAngles = Vector3.Lerp(feedBackCharacter.localEulerAngles, trainingRot, 0.1f);
        }
    }

    void OnMentalCommandRecieved(MentalCommand command)
    {
        print($"Mental command received: {command}");
        if (realtimeFeedback && command.action != "neutral")
            feedbackAnim.SetFloat("brush speed", (float)command.power);
    }

    void OnTrainingComplete(JObject result)
    {
        // if applicable, display grade if applicable and activate accept/deny options
        if(trainingCount >= 4)
        {
            trainingQualityText.text = $"{41}%";
            trainingCompletionOptions.SetActive(true);
        }
        Debug.Log("============== Training completed!");
    }

    public void Continue()
    {
        switch (trainingState)
        {
            case TrainingState.NEUTRAL:
                trainingState = TrainingState.BRUSHING;
                break;
            case TrainingState.BRUSHING:
                trainingState = TrainingState.NEUTRAL;
                feedbackAnim.SetBool("brushing", true);
                trainingCount++;
                break;
        }
        if (trainingCount >= minTrainingRounds)
        {
            validation.SetActive(true);
            training.SetActive(false);
            trainingState = TrainingState.VALIDATION;
            // activate option to finish training
        }
        else
            StartTrainingCountdown();

        realtimeFeedback = trainingCount > 4;
        feedbackAnim.SetBool("brushing", trainingState != TrainingState.NEUTRAL);
    }

    public void StartTrainingCountdown()
    {
        trainingCountdown = true;
        trainingCountdownTimer = trainingCountdownTime;
        countDownText.enabled = true;
        countDownText.text = $"{(int)trainingCountdownTimer}";
    }

    public void AcceptTraining()
    {
        Cortex.training.AcceptTraining(trainingAction);
        trainingCompletionOptions.SetActive(false);
        Continue();
    }

    public void RejectTraining()
    {
        Cortex.training.RejectTraining(trainingAction);
        trainingCompletionOptions.SetActive(false);
        StartTrainingCountdown();
    }

    public void FinishTraining()
    {
        // add new bci input to input handler
    }

    public void ContinueTraining()
    {
        // return to training with button to switch to validation view
        trainingState = TrainingState.NEUTRAL;
        trainingCount = minTrainingRounds - 2;
        Continue();
    }

    public void ResetTraining()
    {
        Cortex.training.EraseTraining("brush");
        Cortex.training.EraseTraining("neutral");

        validation.SetActive(false);
        training.SetActive(true);

        trainingCount = 0;
        trainingState = TrainingState.NEUTRAL;
    }

    public void SkipToValidation()
    {
        returning.SetActive(false);
        feedBackCharacter.gameObject.SetActive(true);
        trainingState = TrainingState.VALIDATION;
        trainingCount = minTrainingRounds;
        Continue();
    }
}