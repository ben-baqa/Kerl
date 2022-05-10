using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public bool brushing;

    private bool throwing;
    private float timer;
    private Skipper skipper;
    private CurlingBar bar;

    private void Start()
    {
        throwing = false;
        brushing = false;
        skipper = FindObjectOfType<Skipper>();
        bar = FindObjectOfType<CurlingBar>(true);
    }

    public void StartTimer() {
        timer = Random.Range(0.0f, skipper.period);
        throwing = true;
    }

    void Update()
    {
        brushing = false;
        if (throwing)
        {
            if (timer <= 0)
            {
                brushing = true;
                throwing = false;
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
        else
        {
            if (bar.progress < 0.75f)
            {
                brushing = true;
            }
        }
    }
}
