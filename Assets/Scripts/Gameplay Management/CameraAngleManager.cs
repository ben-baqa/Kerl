using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraAngleManager : MonoBehaviour
{
    public CameraTransition[] transitions;

    CameraTransition current;
    GameState currentState = GameState.Establishing;
    Transform cameraTransform;

    public event EventHandler<GameState> TransitionComplete;


    void Update()
    {
        if (current == null)
            return;
        if (current.Process())
            TransitionComplete(this, current.targetState);
    }

    public void ApplyGameState(GameState gameState)
    {
        if(!cameraTransform)
            cameraTransform = Camera.main.transform;

        bool transitionFound = false;
        foreach (var transition in transitions)
        {
            if(transition.targetState == gameState && transition.previousState == currentState)
            {
                current = transition;
                if (transition.Start(cameraTransform))
                    TransitionComplete(this, current.targetState);
                transitionFound = true;
            }
        }

        currentState = gameState;
        if (!transitionFound)
            Debug.LogWarning($"No Transition found for {currentState} to {gameState}");
    }
}