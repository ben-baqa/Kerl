using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Scorekeeper : MonoBehaviour
{
    //current score of game, blue is pos, red is neg
    public int score;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
    }

    public void UpdateScore()
    {
        Rock[] rocks = FindObjectsOfType<Rock>();

        rocks = rocks.OrderBy(x => (x.transform.position -
            transform.position).magnitude).ToArray();

        bool blue = rocks[0].blue;
        score = 1;
        for(int i = 1; i < rocks.Length; i++)
        {
            if (rocks[i].blue != blue)
                break;
            score++;
        }

        for (int i = 0; i < rocks.Length; i++)
            rocks[i].Score(score > i);

        if (!blue)
            score *= -1;
    }
}
