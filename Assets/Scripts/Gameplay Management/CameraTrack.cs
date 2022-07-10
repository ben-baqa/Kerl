using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public bool IsComplete => complete;

    public float duration = 5;
    public bool lookAtTrack;
    public bool uniformSpeed;
    public bool startOnAwake;
    public Transform target;
    public Transform lookAt;
    public AnimationCurve curve;

    BezierSpline spline;

    bool complete;
    float progress;
    bool active;

    private void Start()
    {
        spline = GetComponent<BezierSpline>();

        if (startOnAwake)
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
        Process(target);
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
