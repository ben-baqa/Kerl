using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the rock throwing logic
/// </summary>
public class FakeSkipper : MonoBehaviour
{
    enum State { inactive, targeting, throwing }
    State state = State.inactive;

    public float period = 5, maxAngle;
    public bool weightedCurve;

    CurveLine line;
    TurnManager turnManager;
    Rock rock;

    private float n, angle;

    void Start()
    {
        line = GetComponentInChildren<CurveLine>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    void Update()
    {
        if (state == State.targeting)
            RunTargetingLogic();
        if (state == State.throwing)
            RunThrowLogic();
    }

    private void RunThrowLogic()
    {
        angle = Angle();
        line.Generate(angle);

        if (turnManager.GetToggledInput())
        {
            Throw();
        }
    }

    public void Throw()
    {
        state = State.inactive;
        n = 0;
        line.OnThrow();

        //rock.Throw(angle, angle / maxAngle);
        RoundManager.instance.OnThrow();
    }

    private float Angle()
    {
        n += Time.deltaTime;
        if (n > period)
            n -= period;

        float sin = Mathf.Sin((n / period) * 2 * Mathf.PI);
        if (weightedCurve)
            return maxAngle * Mathf.Pow(sin, 3);
        return maxAngle * sin;
    }

    void RunTargetingLogic()
    {
        if(turnManager.GetToggledInput())
        {
            RoundManager.instance.OnTargetSelect();
            state = State.throwing;
        }
    }

    public void StartTargetSelection(Rock selectedRock)
    {
        turnManager.GetToggledInput();
        state = State.targeting;
        rock = selectedRock;
    }
}
