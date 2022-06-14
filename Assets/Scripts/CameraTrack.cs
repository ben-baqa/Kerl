using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTrack : MonoBehaviour
{
    public float duration = 5;
    public float loopTime = 1;
    public bool lookAtTrack;
    public bool uniformSpeed;
    public Transform lookAt;
    public AnimationCurve curve;

    BezierSpline spline;
    Transform cameraTransform;

    float progress;
    bool active;

    private void Start()
    {
        spline = GetComponent<BezierSpline>();
        cameraTransform = Camera.main.transform;

        Activate();
    }

    void Activate()
    {
        active = true;
    }

    private void Update()
    {
        if (!active)
            return;

        Vector3 point = spline.GetPoint(curve.Evaluate(progress));
        if(uniformSpeed)
            point = spline.GetPointByLength(curve.Evaluate(progress));
        cameraTransform.position = point;
        if (lookAtTrack)
            cameraTransform.LookAt(point + spline.GetDirection(curve.Evaluate(progress)));
        else if (lookAt)
            cameraTransform.LookAt(lookAt.position);

        progress += Time.deltaTime / duration;
        
        if (progress > loopTime)
            progress -= loopTime;
    }
}
