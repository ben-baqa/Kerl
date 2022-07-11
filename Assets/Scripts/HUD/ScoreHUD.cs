using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the current score of a game and whose turn it is
/// </summary>
public class ScoreHUD : MonoBehaviour
{
    public Text bScoreOutline, rScoreOutline, endHeader, endLeaveText;
    public Vector3 endScorePos;
    public Color blue, red;
    public Canvas endCanvas;

    public AudioSource blueWinNoise, redWinNoise, tieNoise, appluase, mainMusic;

    private Text bScore, rScore;

    private bool ended = false;

    void Start()
    {
        bScore = bScoreOutline.GetComponentsInChildren<Text>()[1];
        rScore = rScoreOutline.GetComponentsInChildren<Text>()[1];
        endCanvas.enabled = false;
        endLeaveText.enabled = false;

        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
    }

    public void OnResult()
    {
        bScoreOutline.gameObject.SetActive(true);
        rScoreOutline.gameObject.SetActive(true);
    }

    public void OnTurnStart()
    {
        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
    }

    public void UpdateScore(int score)
    {
        if (ended)
            return;
        if(score > 0)
        {
            bScoreOutline.text = bScore.text = "" + score;
            rScoreOutline.text = rScore.text = "0";
        }
        else
        {
            bScoreOutline.text = bScore.text = "0";
            rScoreOutline.text = rScore.text = "" + -score;
        }
    }


    public void ShowEndCard()
    {
        float score = FindObjectOfType<Scorekeeper>().score;

        rScoreOutline.gameObject.SetActive(false);
        bScoreOutline.gameObject.SetActive(true);
        bScoreOutline.rectTransform.localPosition = endScorePos;
        bScore.color = score > 0 ? blue : red;
        bScoreOutline.text = bScore.text = "" + Mathf.Abs(score);

        if (score > 0)
        {
            endHeader.text = "Purple\nTeam\nWins!";
            blueWinNoise.Play();
            appluase.Play();
        }
        else if(score < 0)
        {
            endHeader.text = "Yellow\nTeam\nWins!";
            redWinNoise.Play();
            if (JoinMenu.player_count > 2)
                appluase.Play();
        }
        else
        {
            endHeader.text = "A\nTie!";
            tieNoise.Play();
            bScoreOutline.color = new Color(0, 1, 1);
        }
        mainMusic.Stop();
        endCanvas.enabled = true;
        ended = true;
    }

    public void ShowEndLeaveText()
    {
        endLeaveText.enabled = true;
    }
}
