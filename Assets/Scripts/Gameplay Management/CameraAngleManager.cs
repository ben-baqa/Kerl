using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CameraAngleManager : MonoBehaviour
{
    public CameraTransition[] transitions;

    CameraTransition current;
    GameState currentState;
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

public enum CameraTransitionType { Cut, Lerp, Spline }
[System.Serializable]
public class CameraTransition
{
    public CameraTransitionType type;
    public GameState previousState;
    public GameState targetState;

    public Transform lerpTarget;

    public float duration;

    public CameraTrack track;

    Transform cameraTransform;
    Vector3 startPosition;
    Quaternion startRotation;
    float progress = 0;
    bool complete = false;



    public bool Start(Transform camera)
    {
        cameraTransform = camera;
        complete = false;
        progress = 0;

        if(type == CameraTransitionType.Cut)
        {
            camera.position = lerpTarget.position;
            camera.rotation = lerpTarget.rotation;
            complete = true;
        }
        else if(type == CameraTransitionType.Lerp)
        {
            startPosition = camera.position;
            startRotation = camera.rotation;
        }
        return complete;
    }

    public bool Process()
    {
        if (complete)
            return false;

        progress += Time.deltaTime / duration;

        if(type == CameraTransitionType.Lerp)
        {
            cameraTransform.position = Vector3.Lerp(startPosition, lerpTarget.position, progress);
            cameraTransform.rotation = Quaternion.Lerp(startRotation, lerpTarget.rotation, progress);
        }
        else if(type == CameraTransitionType.Spline)
        {
            track.Process(cameraTransform);
        }
        if (progress >= 1)
            complete = true;
        return complete;
    }
}