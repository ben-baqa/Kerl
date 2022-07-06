using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class TestThrower : MonoBehaviour
{
    enum ThrowState { 
        SelectEnd,
        SelectMid,
        PreThrow,
        PostThrow
    }

    public Vector3 ForwardDirection;
    public Vector3 AimingWidth;

    public LineRenderer CurveRenderer;
    public TestNewRock rock;

    [Range(-1.0f, 1.0f)]
    public float EndPointAim;
    [Range(-1.0f, 1.0f)]
    public float MidpointAim;

    private bool aimMovingRight = true;
    private ThrowState throwState = ThrowState.SelectEnd;

    public void CurveUpdate() {
        int steps = 20;
        for (int i = 0; i <= steps; i++)
        {
            CurveRenderer.SetPosition(i, Bezier.GetPoint(Vector3.zero, GetMidPoint(), GetEndPoint(), (float)i / (float)steps));
        }
    }

    private void Start()
    {
    }

    private void FixedUpdate()
    {
        if (throwState == ThrowState.SelectEnd)
        {
            EndPointAim += Time.deltaTime * (aimMovingRight ? 1 : -1);
            if (Mathf.Abs(EndPointAim) >= 1)
            {
                EndPointAim = aimMovingRight ? 1 : -1;
                aimMovingRight = !aimMovingRight;
            }
            if (Keyboard.current[Key.Space].wasPressedThisFrame) {
                throwState = ThrowState.SelectMid;
                aimMovingRight = true;
            }
            CurveUpdate();
        }
        else if (throwState == ThrowState.SelectMid)
        {
            MidpointAim += Time.deltaTime * (aimMovingRight ? 1 : -1);
            if (Mathf.Abs(MidpointAim) >= 1)
            {
                MidpointAim = aimMovingRight ? 1 : -1;
                aimMovingRight = !aimMovingRight;
            }
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                throwState = ThrowState.PreThrow;
            }
            CurveUpdate();
        }
        else if (throwState == ThrowState.PreThrow)
        {
            if (Keyboard.current[Key.Space].wasPressedThisFrame)
            {
                throwState = ThrowState.PostThrow;
                rock.MidPoint = GetMidPoint();
                rock.EndPoint = GetEndPoint();
                rock.Throw();
            }
        }
    }

    public Vector3 GetEndPoint() {
        return ForwardDirection + AimingWidth * EndPointAim;
    }

    public Vector3 GetMidPoint()
    {
        return ForwardDirection * 0.5f + AimingWidth * MidpointAim;
    }
}

[CustomEditor(typeof(TestThrower))]
public class TestThrowerEditor : Editor
{
    private TestThrower thrower;
    private Transform handleTransform;

    private void OnSceneGUI()
    {
        thrower = target as TestThrower;
        handleTransform = thrower.transform;

        Vector3 direction = handleTransform.position + thrower.ForwardDirection;

        Vector3 start = handleTransform.position;
        Vector3 mid = handleTransform.position + thrower.GetMidPoint();
        Vector3 end = handleTransform.position + thrower.GetEndPoint();

        Handles.color = Color.blue;
        Handles.DrawLine(start, direction);
        Handles.color = Color.green;
        Handles.DrawLine(direction + thrower.AimingWidth, direction - thrower.AimingWidth);

        Handles.color = Color.white;
        Handles.DrawLine(start, mid);
        Handles.color = Color.cyan;
        Handles.DrawLine(mid, end);

        Handles.color = Color.red;
        int steps = 20;
        Vector3 lastPoint = start;
        for (int i = 1; i <= steps; i++)
        {
            Vector3 temp = Bezier.GetPoint(start, mid, end, (float)i / (float)steps);
            Handles.DrawLine(lastPoint, temp);
            lastPoint = temp;
        }
        thrower.CurveUpdate();
    }
}