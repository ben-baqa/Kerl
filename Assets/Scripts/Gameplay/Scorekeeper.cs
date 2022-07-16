using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

/// <summary>
/// Keeps track of the score
/// </summary>
public class Scorekeeper : MonoBehaviour
{
    //current score of game, blue is pos, red is neg
    public int score;
    public float minDistance = 6.75f;

    ScoreHUD display;
    List<Rock> rocks;

    bool awaitingFinalScore;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, .3f);
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }

    void Start()
    {
        display = FindObjectOfType<ScoreHUD>();
        rocks = new List<Rock>();
    }

    void Update()
    {
        UpdateScore();
        display.UpdateScore(score);

        if (awaitingFinalScore)
            WaitForFinalScore();
    }

    public void OnRockSelected(Rock newRock)
    {
        rocks.Add(newRock);
    }

    public void UpdateScore()
    {
        //Rock[] rocks = FindObjectsOfType<Rock>();
        if (rocks.Count < 1)
            return;

        rocks.RemoveAll(rock => rock == null);
        rocks = rocks.OrderBy(x => (x.position - transform.position).magnitude).ToList();

        score = 0;
        bool blue = rocks[0].blue;
        if ((rocks[0].position - transform.position).magnitude < minDistance)
        {
            blue = rocks[0].blue;
            score = 1;
            for (int i = 1; i < rocks.Count; i++)
            {
                if (rocks[i].blue != blue || Vector3.Distance(
                    rocks[i].transform.position, transform.position) > minDistance)
                    break;
                score++;
            }
        }

        for (int i = 0; i < rocks.Count; i++)
            rocks[i].Score(score > i);

        if (!blue)
            score *= -1;
    }

    public void GetFinalResult()
    {
        awaitingFinalScore = true;
    }

    public void WaitForFinalScore()
    {
        foreach (Rock rock in rocks)
            if (rock.IsMoving)
                return;

        awaitingFinalScore = false;
        RoundManager.instance.ShowFinalResult();
    }
}
