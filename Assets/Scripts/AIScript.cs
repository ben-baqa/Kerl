using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    public bool brushing;

    private bool started;
    private float timer;
    private Skipper skipper;
    private CurlingBar bar;

    private void Start()
    {
        started = false;
        brushing = false;
    }

    void StartTimer() {
        skipper = FindObjectOfType<Skipper>();
        timer = Random.Range(0.0f, skipper.period);
        bar = FindObjectOfType<CurlingBar>();
        started = true;
    }

    void Update()
    {
        brushing = false;
        if (started)
        {
            if (timer <= 0)
            {
                if (bar.progress < 0.75f)
                {
                    brushing = true;
                }
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
