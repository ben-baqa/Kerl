using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs simple Ai behavior
/// </summary>
public class AIScript : MonoBehaviour
{
    enum State { Inactive, Timed, Brushing }

    public bool input;

    State state = State.Inactive;

    private float timer;
    private Thrower thrower;
    private CurlingBar bar;

    private void Start()
    {
        input = false;
        thrower = FindObjectOfType<Thrower>();
        bar = FindObjectOfType<CurlingBar>(true);
    }

    public void StartTimer() {
        state = State.Timed;
        timer = Random.Range(0.0f, thrower.aimFrequency);
    }

    void Update()
    {
        input = false;
        if (state == State.Timed)
        {
            if (timer <= 0)
            {
                input = true;
                state = State.Inactive;
            }
            timer -= Time.deltaTime;
        }
        else if (state == State.Brushing)
        {
            input = bar.Progress < 0.75f;
        }
    }

    public void OnResult()
    {
        state = State.Inactive;
    }
}
