using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the rock throwing logic
/// </summary>
public class Thrower : MonoBehaviour
{
    enum State
    {
        Inactive,
        ChoosingEndPoint,
        ChoosingMidPoint
    }

    public float aimFrequency = 0.5f;
    public float Period => 1 / aimFrequency;

    [Header("Throwing settings")]
    public Vector3 throwingDirection;
    public float targetRadius = 1.5f;
    public float sheetWidth = 4;

    [Header("Prediction Curve Settings")]
    public int curveSteps = 100;
    public float curveWidth = 1;

    [Header("Rock Movement Settings")]
    [Tooltip("Ratio of normal rock curve spent brushing")]
    public float brushingRatio = 0.85f;

    Rock rock;
    Sweeper sweeper;
    TurnManager turnManager;
    LineRenderer curve; // need a line renderer with X + 1 points

    float preSnapAim;
    float endPointAim;
    float midPointAim;

    State currentState;

    private void Start()
    {
        sweeper = FindObjectOfType<Sweeper>();
        curve = GetComponentInChildren<LineRenderer>();
        turnManager = FindObjectOfType<TurnManager>();

        curve.positionCount = curveSteps + 1;
        curve.widthMultiplier = curveWidth;
        curve.enabled = false;

        currentState = State.Inactive;

        preSnapAim = 0;
        endPointAim = 0;
        midPointAim = 0;
    }

    void Update()
    {
        if (currentState == State.Inactive)
            return;

        preSnapAim += Mathf.Clamp01(aimFrequency * Time.deltaTime);
        if (preSnapAim >= 1)
            preSnapAim -= 1;

        if (currentState == State.ChoosingEndPoint)
        {
            endPointAim = AimWithSnapping(preSnapAim, 1, 1);
            CurveUpdate(false);

            if (turnManager.GetToggledInput())
            {
                RoundManager.instance.OnTargetSelect();
                currentState = State.ChoosingMidPoint;
                preSnapAim = 0;
            }

        }
        else if (currentState == State.ChoosingMidPoint)
        {
            float leftValue = Mathf.Lerp(.5f, 2, (endPointAim + 1) / 2);
            midPointAim = AimWithSnapping(preSnapAim, leftValue, 2.5f - leftValue);
            CurveUpdate(true);

            if (turnManager.GetToggledInput())
            {
                RoundManager.instance.OnThrow();

                curve.enabled = false;
                currentState = State.Inactive;

                //float notBrushingRatio = 3 * targetRadius / Vector3.Distance(Vector3.zero, GetEndPoint());

                rock.Throw(transform.position, transform.position + GetMidPoint(),
                    transform.position + GetEndPoint(), brushingRatio, midPointAim, targetRadius);

                sweeper.OnThrow(transform.position, throwingDirection, targetRadius, brushingRatio);
            }
        }
    }

    private float AimWithSnapping(float preSnap, float left, float right)
    {
        return (1 - Mathf.Abs(Mathf.Cos(preSnapAim * 2 * Mathf.PI))) * (preSnap > 0.5 ? -left : right);
    }

    public void CurveUpdate(bool isCurve)
    {
        if (isCurve)
        {
            int steps = curveSteps;
            for (int i = 0; i <= steps; i++)
                curve.SetPosition(i, transform.position + Bezier.GetPoint(Vector3.zero, GetMidPoint(), GetEndPoint(), (float)i / (float)steps));
        }
        else
        {
            int steps = curveSteps;
            for (int i = 0; i <= steps; i++)
                curve.SetPosition(i, transform.position + Vector3.Lerp(Vector3.zero, GetEndPoint(), (float)i / (float)steps));
        }
    }

    public Vector3 GetEndPoint()
    {
        return throwingDirection + (Quaternion.Euler(0, 90, 0) * throwingDirection).normalized * targetRadius * endPointAim;
    }

    public Vector3 GetMidPoint()
    {
        return GetEndPoint() * 0.5f + (Quaternion.Euler(0, 90, 0) * GetEndPoint()).normalized * sheetWidth * midPointAim / 2;
    }

    public void StartTargetSelection(Rock selectedRock)
    {
        turnManager.GetToggledInput();
        currentState = State.ChoosingEndPoint;
        rock = selectedRock;
        curve.enabled = true;

        preSnapAim = 0;
        endPointAim = 0;
        midPointAim = 0;
    }
}
