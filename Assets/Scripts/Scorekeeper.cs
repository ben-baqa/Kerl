using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Scorekeeper : MonoBehaviour
{
    //current score of game, blue is pos, red is neg
    public int score;

    private ScoreHUD display;

    // Start is called before the first frame update
    void Start()
    {
        display = FindObjectOfType<ScoreHUD>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateScore();
        display.UpdateScore(score);
    }

    public void UpdateScore()
    {
        Rock[] rocks = FindObjectsOfType<Rock>();
        if (rocks.Length < 1)
            return;

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
