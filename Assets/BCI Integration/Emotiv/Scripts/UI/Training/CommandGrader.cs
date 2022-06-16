using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;

public class CommandGrader : MonoBehaviour
{
    public float grade;

    string headsetID => trainingMenu.headsetID;
    string action = "neutral";
    float timeActive;
    float startTime;
    bool active = false;
    bool grading = false;

    float gradingTime = 8;

    TrainingSubmenu trainingMenu;


    private void Start()
    {
        trainingMenu = GetComponent<TrainingSubmenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (grading)
        {
            gradingTime += Time.deltaTime;
            if (active)
                timeActive += Time.deltaTime;
        }
    }

    void OnMentalCommandRecieved(MentalCommand command)
    {
        active = command.action == action;
        //else if (grading)
        //    print("Sweet, right command.");
    }

    public void StartGrading()
    {
        startTime = Time.time;
        gradingTime = 0;
        timeActive = 0;
        grading = true;
        Cortex.SubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
    }

    public void StartGrading(string actionToGrade)
    {
        action = actionToGrade;
        StartGrading();
    }

    public void FinishGrading()
    {
        Cortex.UnsubscribeMentalCommands(headsetID, OnMentalCommandRecieved);
        grading = false;
        grade =  timeActive / gradingTime;
    }
}
