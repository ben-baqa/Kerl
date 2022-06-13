using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CameraTrack))]
public class CameraTrackInspector : Editor
{
    private void OnSceneGUI()
    {
        CameraTrack track = target as CameraTrack;
        Transform tr = track.transform;

        Handles.color = Color.white;
        Handles.DrawLine(tr.TransformPoint(track.p0), tr.TransformPoint(track.p1));
    }
}
