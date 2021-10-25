using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Turn = TurnManager.Turn;

public class ScoreHUD : MonoBehaviour
{
    public Text playerOutline, bScoreOutline, rScoreOutline, endText;
    public Vector3 startPos, throwPos, endScorePos;
    public Color blue, red;
    public Canvas endCanvas;

    public float sizeDelta = .2f, sizeFreq = 5;

    public AudioSource blueWinNoise, redWinNoise, tieNoise, appluase, mainMusic;

    private Text player, bScore, rScore;

    private float size = 1;
    private bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        player = playerOutline.GetComponentsInChildren<Text>()[1];
        bScore = bScoreOutline.GetComponentsInChildren<Text>()[1];
        rScore = rScoreOutline.GetComponentsInChildren<Text>()[1];
        endCanvas.enabled = false;
    }

    private void Update()
    {
        size = 1 + Mathf.Sin(Time.time * sizeFreq) * sizeDelta;

        playerOutline.transform.localScale = Vector3.one * size;
    }

    public void OnThrow()
    {
        //playerOutline.gameObject.SetActive(false);
        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
        playerOutline.rectTransform.localPosition = throwPos;
    }

    public void OnResult()
    {
        playerOutline.gameObject.SetActive(false);
        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
    }

    public void UpdateTurn(Turn turn)
    {
        string s = "";
        Color c = blue;
        switch (turn)
        {
            case Turn.p1:
                s = "P1";
                break;
            case Turn.p2:
                s = "P2";
                break;
            case Turn.p3:
                s = "P3";
                c = red;
                break;
            case Turn.p4:
                s = "P4";
                c = red;
                break;
            case Turn.com:
                s = "AI";
                c = red;
                break;
        }
        playerOutline.text = player.text = s;
        player.color = c;
        playerOutline.rectTransform.localPosition = startPos;

        playerOutline.gameObject.SetActive(true);
        bScoreOutline.gameObject.SetActive(true);
        rScoreOutline.gameObject.SetActive(true);
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

    public void EndGame()
    {
        StartCoroutine(ShowEndCard());
    }

    private IEnumerator ShowEndCard()
    {
        yield return new WaitForSeconds(3);
        float score = FindObjectOfType<Scorekeeper>().score;

        bScoreOutline.gameObject.SetActive(true);
        bScoreOutline.rectTransform.localPosition = endScorePos;
        bScore.color = score > 0 ? blue : red;
        bScoreOutline.text = bScore.text = "" + Mathf.Abs(score);

        if (score > 0)
        {
            endText.text = "Purple\nTeam\nWins!";
            blueWinNoise.Play();
            appluase.Play();
        }
        else if(score < 0)
        {
            endText.text = "Yellow\nTeam\nWins!";
            redWinNoise.Play();
            if (SelectCharacter.player_count > 2)
                appluase.Play();
        }
        else
        {
            endText.text = "A\nTie!";
            tieNoise.Play();
            bScoreOutline.color = new Color(0, 1, 1);
        }
        mainMusic.Stop();
        endCanvas.enabled = true;
        ended = true;
    }
}
