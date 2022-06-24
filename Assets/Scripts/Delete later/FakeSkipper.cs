using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the rock throwing logic
/// </summary>
public class FakeSkipper : MonoBehaviour
{
    enum State { idle, targeting, throwing }
    State state = State.idle;

    public float period = 5, maxAngle;
    public bool weightedCurve;

    // TODO: change Jersey switching to character swap
    public Material blueTeam, redTeam;

    private Animator anim;
    private CurveLine line;
    private SkinnedMeshRenderer rend;
    private TurnManager turnManager;
    private Rock rock;

    private float n, angle;

    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        line = GetComponentInChildren<CurveLine>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        turnManager = FindObjectOfType<TurnManager>();
        StartTurn(false);
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

        if (turnManager.GetInput())
        {
            Throw();
        }
    }

    public void Throw()
    {
        state = State.idle;
        n = 0;
        line.OnThrow();

        anim.SetTrigger("push");
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
            RoundManager.instance.OnTargetSelect();
            state = State.throwing;
        }
    }

    public void StartTurn(bool blueTurn)
    {
        //state = State.idle;

        rend.material = blueTurn ? blueTeam : redTeam;
    }

    public void StartTargetSelection(Rock selectedRock)
    {
        state = State.targeting;
        rock = selectedRock;
    }
}
