using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public bool IsComplete => complete;

    public float duration = 5;
    public bool lookAtTrack;
    public bool uniformSpeed;
    public Transform lookAt;
    public AnimationCurve curve;

    BezierSpline spline;
    Transform cameraTransform;

    bool complete;
    float progress;
    bool active;

    private void Start()
    {
        spline = GetComponent<BezierSpline>();
        cameraTransform = Camera.main.transform;

        Activate();
    }

    public void Activate()
    {
        active = true;
    }

    private void Update()
    {
        if (!active)
            return;

        Process();
    }

    public void Process()
    {
        Process(cameraTransform);
    }

    public void Process(Transform target)
    {
        if (complete)
            return;

        Vector3 point = spline.GetPoint(curve.Evaluate(progress));
        if(uniformSpeed)
            point = spline.GetPointByLength(curve.Evaluate(progress));
        target.position = point;
        if (lookAtTrack)
            target.LookAt(point + spline.GetDirection(curve.Evaluate(progress)));
        else if (lookAt)
            target.LookAt(lookAt.position);

        if (progress >= 1)
            complete = true;

        progress += Time.deltaTime / duration;
    }
}
