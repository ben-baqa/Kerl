using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
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

    [Header("settings")]
    public int minRounds = 11;
    public int maxRounds = 16;
    public int assistRounds = 6;
    public int feedbackThreshold = 4;
    public int countdownTime = 4;
    [Header("References")]
    public GameObject trainingCompletionOptions;
    public GameObject earlyCompletionOption;
    public GameObject completionButton;
    public GameObject backOptions;

    //public TextMeshProUGUI trainingQualityText;
    public TextMeshProUGUI countDownText;
    public TextMeshProUGUI upNextText;
    public TextMeshProUGUI commandText;
    public TextMeshProUGUI failureText;
    public Animator feedbackAnim;

    public ProgressBar trainingProgressBar;
    public ProgressBar progressBar;

    public Action OnTrainingComplete;

    TrainingGradeDisplay gradeDisplay;

    [HideInInspector]
    public string headsetID;
    [HideInInspector]
    public int roundsTrained;

    private string profileName;
    float timer = 0;
    int trainingRounds = 0;
    bool feedbackEnabled = false;
    bool completionEnabled = false;
    bool trainingCountdown = false;
    bool returning = false;

    float assistance
    {
        get
        {
            if (trainingRounds < feedbackThreshold)
                return 1;
            else if (trainingRounds < feedbackThreshold + assistRounds)
                return 1 - ((trainingRounds - feedbackThreshold) / assistRounds);
            return 0;
        }
    }

    public void Init()
    {
        trainingCompletionOptions.SetActive(false);
        earlyCompletionOption.SetActive(false);

        gradeDisplay = GetComponentInChildren<TrainingGradeDisplay>(true);

        trainingProgressBar.Init();
        progressBar.Init();
        completionButton.SetActive(false);
        backOptions.SetActive(false);
        commandText.text = "";

        gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        Cortex.training.TrainingThresholdResult += OnTrainingThresholdResult;
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        Cortex.SubscribeSysEvents(headsetID, OnSysEventReceived);
        Cortex.training.ProfileLoaded += (string s) => profileName = s;

        // disables background scrolling with cursor
        CursorOffset.active = false;
        failureText.enabled = false;
    }
    public void OnDisable()
    {
        Cortex.training.TrainingThresholdResult -= OnTrainingThresholdResult;
        Cortex.UnsubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        Cortex.UnsubscribeSysEvents(headsetID, OnSysEventReceived);

        CursorOffset.active = true;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = 0;
            if (trainingCountdown) // start training
            {
                Cortex.training.StartTraining(action);
                trainingCountdown = false;
            }
        }
        countDownText.text = trainingCountdown ? $"{(int)timer + 1}" : "";
        trainingProgressBar.SetProgress((8 - timer) / 8f);
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

    // when a result is recieved after asking for threshold following training success
    void OnTrainingThresholdResult(TrainingThreshold args)
    {
        gradeDisplay.SetGrade((float)args.lastTrainingScore);
        //trainingQualityText.text = $"{(int)(args.lastTrainingScore * 100)}%";

        if (feedbackEnabled && trainingState == TrainingState.BRUSHING)
            trainingCompletionOptions.SetActive(true);
        else
            earlyCompletionOption.SetActive(true);
    }

    void OnSysEventReceived(SysEventArgs args)
    {
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
        failureText.enabled = false;
        bool neutral = trainingState == TrainingState.NEUTRAL;
        commandText.text = neutral ? "relax" : "brush!";

        trainingProgressBar.Activate();
        timer = 8;
    }

    // when training stage completes with a success
    void OnTrainingSucceeded()
    {
        Cortex.training.GetTrainingThreshold();
        countDownText.enabled = false;
        timer = Mathf.Infinity;
        commandText.text = "";
        trainingProgressBar.Deactivate();
        if (!returning)
            progressBar.Activate();
    }

    // when training stage completed with a failure
    void OnTrainingFail()
    {
        commandText.text = "";
        countDownText.enabled = false;
        failureText.enabled = true;
        timer = Mathf.Infinity;
        ActivateUpNext();
        ApplyState();
    }

    // when training was successfully rejected
    void OnTrainingRejected()
    {
        ActivateUpNext();
    }

    // when training has been started, succeeded, and accepted
    void OnTrainingCompleted()
    {
        switch (trainingState)
        {
            case TrainingState.NEUTRAL:
                trainingState = TrainingState.BRUSHING;
                progressBar.SetProgress((trainingRounds + 0.5f) / maxRounds);
                break;
            case TrainingState.BRUSHING:
                trainingState = TrainingState.NEUTRAL;
                trainingRounds++;
                completionEnabled = trainingRounds >= minRounds;
                progressBar.SetProgress((float)trainingRounds / maxRounds);
                break;
        }

        ActivateUpNext();


        ApplyState();
    }

    void ApplyState()
    {
        feedbackEnabled = trainingRounds >= 4;
        feedbackAnim.SetBool("brushing", trainingState != TrainingState.NEUTRAL);
        completionButton.SetActive(completionEnabled);
    }

    void ActivateUpNext()
    {
        trainingProgressBar.Deactivate();
        upNextText.gameObject.SetActive(true);
        upNextText.text = "Ready to\n" +
            (trainingState == TrainingState.NEUTRAL ? "relax" : "brush") +
            "?";
    }

    public void StartTrainingCountdown()
    {
        trainingCountdown = true;
        timer = countdownTime;
        countDownText.enabled = true;
        countDownText.text = $"{(int)timer}";
        upNextText.gameObject.SetActive(false);
        completionButton.SetActive(false);
        progressBar.Deactivate();
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
        //ActivateUpNext();
    }

    // called by overseer script
    public void ResetTraining()
    {
        gameObject.SetActive(true);
        progressBar.gameObject.SetActive(true);
        trainingState = TrainingState.NEUTRAL;
        completionEnabled = false;
        returning = false;
        
        trainingRounds = roundsTrained;
        progressBar.SetProgress((float)trainingRounds / maxRounds);

        ApplyState();
        ActivateUpNext();
    }
    // called by overseer script
    public void ResumeTraining()
    {
        gameObject.SetActive(true);
        progressBar.gameObject.SetActive(false);
        trainingState = TrainingState.NEUTRAL;
        completionEnabled = true;
        returning = true;
        trainingRounds = minRounds;
        ApplyState();
        ActivateUpNext();
    }
}
