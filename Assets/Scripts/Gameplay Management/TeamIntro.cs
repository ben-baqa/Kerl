using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamIntro : MonoBehaviour
{
    public float duration = 5;
    public event EventHandler Complete;

    float timer;
    bool active;

    void Update()
    {
        if (active)
        {
            timer += Time.deltaTime;

            // TODO: add actual intro sequence


            if(timer >= duration)
            {
                active = false;
                Complete(this, EventArgs.Empty);
            }
        }
    }

    public void StartSequence()
    {
        timer = 0;
        active = true;
    }
}
