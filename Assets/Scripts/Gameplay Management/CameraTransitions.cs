using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CameraTransitionType { Cut, Lerp, Spline, None }
public enum CameraTransitionLerpType { Linear, EaseIn, Easeout, EaseBoth }
[System.Serializable]
public class CameraTransition
{
    [Header("Transition Type")]
    public CameraTransitionType type;
    public CameraTransitionLerpType lerpType;
    [Header("States")]
    public GameState previousState;
    public GameState targetState;

    [Header("Settings")]
    public Transform lerpTarget;

    public float duration;

    public CameraTrack track;

    Transform cameraTransform;
    Vector3 startPosition;
    Quaternion startRotation;
    float progress = 0;
    float scaledProgress;
    bool complete = false;



    public bool Start(Transform camera)
    {
        cameraTransform = camera;
        complete = false;
        progress = 0;

        if (type == CameraTransitionType.None)
            complete = true;
        else if (type == CameraTransitionType.Cut)
        {
            camera.position = lerpTarget.position;
            camera.rotation = lerpTarget.rotation;
            complete = true;
        }
        else if (type == CameraTransitionType.Lerp)
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

        if (type == CameraTransitionType.Lerp)
        {
            if (lerpType == CameraTransitionLerpType.Linear)
                scaledProgress = progress;
            else if (lerpType == CameraTransitionLerpType.EaseIn)
                scaledProgress = Mathf.Pow(progress, 3);
            else if (lerpType == CameraTransitionLerpType.Easeout)
                scaledProgress = 1 - Mathf.Pow(1 - progress, 3);
            else if (lerpType == CameraTransitionLerpType.EaseBoth)
            {
                float a = Mathf.Pow(progress, 3);
                float b = 1 - Mathf.Pow(1 - progress, 3);
                scaledProgress = Mathf.Lerp(a, b, progress);
            }

            cameraTransform.position = Vector3.Lerp(startPosition, lerpTarget.position, scaledProgress);
            cameraTransform.rotation = Quaternion.Lerp(startRotation, lerpTarget.rotation, scaledProgress);

            if (progress >= 1)
                complete = true;
        }
        else if (type == CameraTransitionType.Spline)
        {
            track.Process(cameraTransform);
            complete = track.IsComplete;
        }
        return complete;
    }
}

[System.Serializable]
public class SimpleCameraTransition
{
    [Header("Lerp Type")]
    public CameraTransitionLerpType lerpType;

    [Header("Settings")]
    public Transform lerpTarget;
    public float duration;

    Transform cameraTransform;
    Vector3 startPosition;
    Quaternion startRotation;
    float progress = 0;
    float scaledProgress;
    bool complete = false;

    public void Start(Transform camera)
    {
        cameraTransform = camera;
        complete = false;
        progress = 0;

        startPosition = camera.position;
        startRotation = camera.rotation;
    }

    public bool Process()
    {
        if (complete)
            return false;

        progress += Time.deltaTime / duration;

        if (lerpType == CameraTransitionLerpType.Linear)
            scaledProgress = progress;
        else if (lerpType == CameraTransitionLerpType.EaseIn)
            scaledProgress = Mathf.Pow(progress, 3);
        else if (lerpType == CameraTransitionLerpType.Easeout)
            scaledProgress = 1 - Mathf.Pow(1 - progress, 3);
        else if (lerpType == CameraTransitionLerpType.EaseBoth)
        {
            float a = Mathf.Pow(progress, 3);
            float b = 1 - Mathf.Pow(1 - progress, 3);
            scaledProgress = Mathf.Lerp(a, b, progress);
        }

        cameraTransform.position = Vector3.Lerp(startPosition, lerpTarget.position, scaledProgress);
        cameraTransform.rotation = Quaternion.Lerp(startRotation, lerpTarget.rotation, scaledProgress);

        if (progress >= 1)
            complete = true;

        return complete;
    }
}
