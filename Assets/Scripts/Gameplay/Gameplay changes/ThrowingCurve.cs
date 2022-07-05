using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ThrowingCurve : MonoBehaviour
{
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public Vector3 MidPoint;

    public Vector3 GetPoint(float f) {
        return transform.TransformPoint(Bezier.GetPoint(StartPoint, MidPoint, EndPoint, f));
    }
}

[CustomEditor(typeof(ThrowingCurve))]
public class ThrowingCurveEditor : Editor
{
    private ThrowingCurve curve;
    private Transform handleTransform;
    private Quaternion handleRotation;

    private void OnSceneGUI()
    {
        curve = target as ThrowingCurve;
        handleTransform = curve.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
            handleTransform.rotation : Quaternion.identity;

        Vector3 p0 = handleTransform.TransformPoint(curve.StartPoint);
        Vector3 p1 = handleTransform.TransformPoint(curve.EndPoint);

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
    }
}
