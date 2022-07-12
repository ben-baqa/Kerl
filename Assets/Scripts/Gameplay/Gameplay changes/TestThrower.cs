using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class TestThrower : MonoBehaviour
{
    public enum ThrowState { 
        SelectEnd,
        SelectMid,
        PreThrow,
        Brushing,
        End
    }

    private bool lastSignal = false;

    public Vector3 ForwardDirection;
    public float TargetDiameter;

    public LineRenderer CurveRenderer;
    public TestNewRock Rock;

    private float endPointAim;
    private float midPointAim;

    private bool aimMovingRight = true;
    public ThrowState throwState = ThrowState.SelectEnd;

    public void CurveUpdate(bool isCurve) {
        if (isCurve)
        {
            int steps = 20;
            for (int i = 0; i <= steps; i++)
            {
                CurveRenderer.SetPosition(i, Bezier.GetPoint(Vector3.zero, GetMidPoint2(), GetEndPoint(), (float)i / (float)steps));
            }
        }
        else {
            int steps = 20;
            for (int i = 0; i <= steps; i++)
            {
                CurveRenderer.SetPosition(i, Vector3.Lerp(Vector3.zero, GetEndPoint(), (float)i / (float)steps));
            }
        }
    }

    private void Start()
    {
        endPointAim = 0;
        midPointAim = 0;
    }

    private void FixedUpdate()
    {
        if (TestInput.GetInstance().GetSignal())
        {
            if (throwState == ThrowState.SelectEnd && !lastSignal)
            {
                throwState = ThrowState.SelectMid;
                aimMovingRight = true;
            }
            else if (throwState == ThrowState.SelectMid && !lastSignal)
            {
                throwState = ThrowState.PreThrow;
            }
            else if (throwState == ThrowState.PreThrow && !lastSignal)
            {
                throwState = ThrowState.Brushing;
                Rock.Throw(transform.position, GetMidPoint2(), GetEndPoint());
                TestInput.GetInstance().StraightInput = true;

                CurveRenderer.enabled = false;
            }
        }

        lastSignal = TestInput.GetInstance().GetSignal();

        if (throwState == ThrowState.SelectEnd)
        {
            endPointAim += Time.deltaTime * (aimMovingRight ? 1 : -1);
            if (Mathf.Abs(endPointAim) >= 1)
            {
                endPointAim = aimMovingRight ? 1 : -1;
                aimMovingRight = !aimMovingRight;
            }
            CurveUpdate(false);
        }
        else if (throwState == ThrowState.SelectMid)
        {
            midPointAim += Time.deltaTime * (aimMovingRight ? 1 : -1);
            if (Mathf.Abs(midPointAim) >= 1)
            {
                midPointAim = aimMovingRight ? 1 : -1;
                aimMovingRight = !aimMovingRight;
            }
            CurveUpdate(true);
        }
    }

    public Vector3 GetEndPoint() {
        return ForwardDirection + (Quaternion.Euler(0, -90, 0) * ForwardDirection).normalized * (TargetDiameter / 2) * endPointAim;
    }

    public Vector3 GetMidPoint2()
    {
        return GetEndPoint() * 0.5f + (Quaternion.Euler(0, -90, 0) * GetEndPoint()).normalized * (TargetDiameter / 2) * midPointAim;
    }
}
