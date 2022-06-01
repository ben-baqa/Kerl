using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using Newtonsoft.Json.Linq;
using EmotivUnityPlugin;

/* Handles the logic of actively training a profile, giving live feedback after the first few rounds.
 * The process of which is as follows:
 * 
 * each round of training is comprised of 2 stages, a neutral relaxed state, and an active input state,
 * each stage consists of a short countdown followed by an eight second period of training,
 * after which the system will query cortex for the quality of the training (if enough rounds have been finished)
 * and display this quality to the user, informing a choice to accept or deny the training.
 * if the player accepts, move on, if not, redo the current stage
 * 
 * If not enough rounds have been completed to warrant the display of training quality,
 * and acceptance of the stage, a simple continue button will be shown, which advances to the next stage.
 */
public class TrainingSubmenu : MonoBehaviour, IRequiresInit
{
    enum TrainingState { NEUTRAL, BRUSHING, VALIDATION }
    TrainingState trainingState = TrainingState.NEUTRAL;

    string action => trainingState == TrainingState.BRUSHING ? "push" : "neutral";

    public int minTrainingRounds = 16, assistRounds = 6,
        feedbackThreshold = 4, countdownTime = 4;
    public GameObject trainingCompletionOptions, earlyCompletionOption, brushPopup, relaxPopup;

    public TextMeshProUGUI trainingQualityText, countDownText;
    public Animator feedbackAnim;

    public Action OnTrainingComplete;

    [HideInInspector]
    public string headsetID;
    float timer = 0;
    int trainingCount = 0;
    bool feedbackEnabled = false, trainingCountdown = false;

    float assistance
    {
        get {
            if (trainingCount < feedbackThreshold)
                return 1;
            else if (trainingCount < feedbackThreshold + assistRounds)
                return 1 - ((trainingCount - feedbackThreshold) / assistRounds);
            return 0;
        }
    }
    
    public void Init()
    {
        trainingCompletionOptions.SetActive(false);
        earlyCompletionOption.SetActive(false);
        brushPopup.SetActive(false);
        relaxPopup.SetActive(false);
        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        Cortex.training.TrainingThresholdResult += OnTrainingThresholdResult;
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        Cortex.SubscribeSysEvents(headsetID, OnSysEventReceived);
    }
    public void OnDisable()
    {
        Cortex.training.TrainingThresholdResult -= OnTrainingThresholdResult;
        Cortex.UnsubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        Cortex.UnsubscribeSysEvents(headsetID, OnSysEventReceived);
    }

    void Update()
    {
        if(timer > 0)
        {
            timer -= Time.deltaTime;
        } else
        {
            timer = 0;
            if (trainingCountdown) // start training
            {
                Cortex.training.StartTraining(action);
                trainingCountdown = false;
            }
        }
        countDownText.text = $"{(int)timer}";
    }

    void OnMentalCommandRecieved(MentalCommand command)
    {
        if (trainingState == TrainingState.BRUSHING)
        {
            if (command.action == "neutral")
                feedbackAnim.SetFloat("brush speed", assistance);
            else
                feedbackAnim.SetFloat("brush speed", (float)command.power + assistance);
        }
        else
            feedbackAnim.SetFloat("brush speed", 1);
    }

    void OnTrainingThresholdResult(TrainingThreshold args)
    {
        trainingQualityText.text = $"{(int)(args.lastTrainingScore * 100)}%";

        if(feedbackEnabled && trainingState == TrainingState.BRUSHING)
            trainingCompletionOptions.SetActive(true);
        else
            earlyCompletionOption.SetActive(true);
    }

    void OnSysEventReceived(SysEventArgs args)
    {
        print($"System event received: {args.eventMessage}");
        switch (args.eventMessage)
        {
            case "MC_Started":
                OnTrainingStart();
                break;
            case "MC_Succeeded":
                OnTrainingSucceeded();
                break;
            case "MC_Failed":
                OnTrainingFail();
                break;
            case "MC_Completed":
                OnTrainingCompleted();
                break;
            case "MC_Rejected":
                OnTrainingRejected();
                break;
            default:
                print($"Unhandled system message: {args.eventMessage}");
                break;
        }
    }

    // when training has been successfully initiated
    void OnTrainingStart()
    {
        brushPopup.SetActive(trainingState == TrainingState.BRUSHING);
        relaxPopup.SetActive(trainingState == TrainingState.NEUTRAL);
        timer = 8;
    }

    // when training stage completes with a success
    void OnTrainingSucceeded()
    {
        Cortex.training.GetTrainingThreshold();
        countDownText.enabled = false;
        timer = Mathf.Infinity;
        brushPopup.SetActive(false);
        relaxPopup.SetActive(false);
    }

    // when training stage completed with a failure
    void OnTrainingFail()
    {
        Cortex.training.GetTrainingThreshold();
        countDownText.enabled = false;
        timer = Mathf.Infinity;
        brushPopup.SetActive(false);
        relaxPopup.SetActive(false);
    }

    // when training was successfully rejected
    void OnTrainingRejected()
    {
        brushPopup.SetActive(trainingState == TrainingState.BRUSHING);
        relaxPopup.SetActive(trainingState == TrainingState.NEUTRAL);
        StartTrainingCountdown();
    }

    // when training has been started, succeeded, and accepted/rejected
    void OnTrainingCompleted()
    {
        switch (trainingState)
        {
            case TrainingState.NEUTRAL:
                trainingState = TrainingState.BRUSHING;
                break;
            case TrainingState.BRUSHING:
                trainingState = TrainingState.NEUTRAL;
                trainingCount++;
                break;
        }
        if (trainingCount >= minTrainingRounds)
        {
            trainingState = TrainingState.VALIDATION;
            Debug.Log("-------------- Training Sequence Complete");
            // activate option to finish training
            OnTrainingComplete();
        }
        else
            StartTrainingCountdown();

        ApplyState();
    }

    void ApplyState()
    {
        feedbackEnabled = trainingCount >= 4;
        feedbackAnim.SetBool("brushing", trainingState != TrainingState.NEUTRAL);
    }

    public void StartTrainingCountdown()
    {
        trainingCountdown = true;
        timer = countdownTime;
        countDownText.enabled = true;
        countDownText.text = $"{(int)timer}";
    }

    // called by in engine UI
    public void AcceptTraining()
    {
        Cortex.training.AcceptTraining(action);
        trainingCompletionOptions.SetActive(false);
        earlyCompletionOption.SetActive(false);
    }
    // called by in engine UI
    public void RejectTraining()
    {
        Cortex.training.RejectTraining(action);
        trainingCompletionOptions.SetActive(false);
        earlyCompletionOption.SetActive(false);
        StartTrainingCountdown();
    }

    // called by overseer script
    public void ResetTraining(int rounds = 0)
    {
        gameObject.SetActive(true);
        trainingCount = rounds > 0? minTrainingRounds - rounds: 0;
        trainingState = TrainingState.NEUTRAL;
        ApplyState();
        StartTrainingCountdown();
    }
}
