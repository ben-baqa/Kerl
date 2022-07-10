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

    // TODO: change Jersey switching to character swap
    //public Material blueTeam, redTeam;

    //Animator anim;
    CurveLine line;
    //SkinnedMeshRenderer rend;
    TurnManager turnManager;
    Rock rock;

    private float n, angle;

    void Start()
    {
        //anim = GetComponentInChildren<Animator>();
        line = GetComponentInChildren<CurveLine>();
        //rend = GetComponentInChildren<SkinnedMeshRenderer>();
        turnManager = FindObjectOfType<TurnManager>();
        //OnTurnStart(false);
    }

    // Update is called once per frame
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
        //print("throw called");
        state = State.inactive;
        n = 0;
        line.OnThrow();

        //anim.SetTrigger("push");
        rock.Throw(angle, angle / maxAngle);
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
            //print("target selected");
            RoundManager.instance.OnTargetSelect();
            state = State.throwing;
        }
    }

    //public void OnTurnStart(bool blueTurn)
    //{
    //    rend.material = blueTurn ? blueTeam : redTeam;
    //}

    public void StartTargetSelection(Rock selectedRock)
    {
        //print("target select started");
        turnManager.GetToggledInput();
        state = State.targeting;
        rock = selectedRock;
    }
}
