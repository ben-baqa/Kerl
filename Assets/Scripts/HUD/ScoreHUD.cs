using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Displays the current score of a game and whose turn it is
/// </summary>
public class ScoreHUD : MonoBehaviour
{
    public TextMeshProUGUI blueScore;
    public TextMeshProUGUI redScore;
    public TextMeshProUGUI winnerText;

    public Vector3 endScorePos;

    public AudioSource blueWinNoise, redWinNoise, tieNoise, appluase, mainMusic;


    private bool ended = false;

    void Start()
    {
        Hide();
    }

    public void OnResult()
    {
        blueScore.enabled = redScore.enabled = true;
    }

    public void OnTurnStart()
    {
        blueScore.enabled = redScore.enabled = false;
    }

    public void UpdateScore(int score)
    {
        if (ended)
            return;
        if(score > 0)
        {
            blueScore.text = "" + score;
            redScore.text = "0";
        }
        else
        {
            blueScore.text = "0";
            redScore.text = "" + -score;
        }
    }

    public void ShowWinner()
    {
        int score = FindObjectOfType<Scorekeeper>().score;
        UpdateScore(score);

        if(score > 0)
        {
            winnerText.text = "Blue\nTeam\nWins";
            blueWinNoise.Play();
            appluase.Play();
        }else if(score < 0)
        {
            winnerText.text = "Red\nTeam\nWins";
            redWinNoise.Play();
            appluase.Play();
        }
        else
        {
            winnerText.text = "A\n Tie!";
            tieNoise.Play();
        }
        winnerText.enabled = true;

        mainMusic.Stop();
        ended = true;
    }

    public void Hide()
    {
        blueScore.enabled = redScore.enabled = winnerText.enabled = false;
    }
}
