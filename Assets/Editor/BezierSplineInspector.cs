using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BezierSpline))]
public class BezierCurveInspector : Editor
{
    BezierSpline spline;
    Transform handleTransform;
    Quaternion handleRotation;

    static Color[] modeColours = { Color.white, Color.yellow, Color.cyan };

    const int stepsPerCurve = 10;
    const float directionScale = 0.4f;
    const float handleSize = 0.04f;
    const float pickSize = 0.06f;

    int selectedIndex = -1;

    private void OnSceneGUI()
    {
        spline = target as BezierSpline;
        handleTransform = spline.transform;
        handleRotation = Tools.pivotRotation == PivotRotation.Local ?
        handleTransform.rotation: Quaternion.identity;

        Vector3 p0 = ShowPoint(0);
        for(int i = 1; i < spline.ControlPointCount; i += 3)
        {
            Vector3 p1 = ShowPoint(i);
            Vector3 p2 = ShowPoint(i + 1);
            Vector3 p3 = ShowPoint(i + 2);

            Handles.color = Color.gray;
            Handles.DrawLine(p0, p1);
            Handles.DrawLine(p2, p3);

            Handles.DrawBezier(p0, p3, p1, p2, Color.white, null, 2);
            p0 = p3;
        }
        ShowDirections();
    }

    public override void OnInspectorGUI()
    {
        spline = target as BezierSpline;

        EditorGUI.BeginChangeCheck();
        bool loop = EditorGUILayout.Toggle("Loop", spline.Loop);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Toggle Loop");
            spline.Loop = loop;
            EditorUtility.SetDirty(spline);
        }

        if (selectedIndex >= 0 && selectedIndex < spline.ControlPointCount)
            DrawSelectedPointInspector();

        if(GUILayout.Button("Add Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.AddCurve();
            EditorUtility.SetDirty(spline);
        }

        if(GUILayout.Button("Remove Curve"))
        {
            Undo.RecordObject(spline, "Add Curve");
            spline.RemoveEndCurve();
            EditorUtility.SetDirty(spline);
        }
    }

    void DrawSelectedPointInspector()
    {
        GUILayout.Label("Selected Point");
        EditorGUI.BeginChangeCheck();
        Vector3 point = EditorGUILayout.Vector3Field("Position", spline[selectedIndex]);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Move Point");
            EditorUtility.SetDirty(spline);
            spline[selectedIndex] = point;
        }

        EditorGUI.BeginChangeCheck();
        BezierControlPointMode mode = (BezierControlPointMode)
            EditorGUILayout.EnumPopup("Mode", spline.GetControlPointMode(selectedIndex));
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(spline, "Change Point Mode");
            spline.SetControlPointMode(selectedIndex, mode);
            EditorUtility.SetDirty(spline);
        }

        if (GUILayout.Button("Delete Point"))
        {
            Undo.RecordObject(spline, "Delete Point");
            spline.DeletePoint(selectedIndex);
            EditorUtility.SetDirty(spline);
        }
    }

    private void ShowDirections()
    {
        Handles.color = Color.green;
        Vector3 point = spline.GetPoint(0f);
        Handles.DrawLine(point, point + spline.GetDirection(0f) * directionScale);

        int steps = stepsPerCurve * spline.CurveCount;
        for (int i = 1; i <= steps; i++)
        {
            point = spline.GetPoint(i / (float)steps);
            Handles.DrawLine(point, point + spline.GetDirection(i / (float)steps) * directionScale);
        }
    }

    private Vector3 ShowPoint(int index)
    {
        Vector3 point = handleTransform.TransformPoint(spline[index]);

        float size = HandleUtility.GetHandleSize(point);
        if (index == 0)
            size *= 2;
        Handles.color = modeColours[(int)spline.GetControlPointMode(index)];
        if (Handles.Button(point, handleRotation, size * handleSize, size * pickSize, Handles.DotHandleCap))
        {
            Repaint();
            selectedIndex = index;
        }

        if (selectedIndex == index)
        {
            EditorGUI.BeginChangeCheck();
            point = Handles.DoPositionHandle(point, handleRotation);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(spline, "Move Point");
                EditorUtility.SetDirty(spline);
                spline[index] = handleTransform.InverseTransformPoint(point);
            }
        }
        return point;
    }
}
