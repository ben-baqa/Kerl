using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the current score of a game and whose turn it is
/// </summary>
public class ScoreHUD : MonoBehaviour
{
    public Text bScoreOutline, rScoreOutline, endText;
    public Vector3 startPos, throwPos, endScorePos;
    public Color blue, red;
    public Canvas endCanvas;

    public float sizeDelta = .2f, sizeFreq = 5;

    public AudioSource blueWinNoise, redWinNoise, tieNoise, appluase, mainMusic;

    private Text bScore, rScore;
    InputIcon inputIcon;

    private float size = 1;
    private bool ended = false;

    // Start is called before the first frame update
    void Start()
    {
        bScore = bScoreOutline.GetComponentsInChildren<Text>()[1];
        rScore = rScoreOutline.GetComponentsInChildren<Text>()[1];
        inputIcon = GetComponentInChildren<InputIcon>(true);
        endCanvas.enabled = false;
    }

    private void Update()
    {
        size = 1 + Mathf.Sin(Time.time * sizeFreq) * sizeDelta;

        inputIcon.transform.localScale = Vector3.one * size;
    }

    public void OnThrow()
    {
        //playerOutline.gameObject.SetActive(false);
        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
        //playerOutline.rectTransform.localPosition = throwPos;

        inputIcon.rectTransform.localPosition = throwPos;
    }

    public void OnResult()
    {
        //playerOutline.gameObject.SetActive(false);
        inputIcon.gameObject.SetActive(false);
        bScoreOutline.gameObject.SetActive(false);
        rScoreOutline.gameObject.SetActive(false);
    }

    public void UpdateTurn(int turn)
    {
        string s = turn < 0? "AI": $"P{turn + 1}";
        Color c = MenuSelections.GetColor(turn);

        if (turn >= 0)
            inputIcon.Index = turn;
        else
            inputIcon.text.text = "AI";
        inputIcon.image.enabled = turn >= 0;

        inputIcon.rectTransform.localPosition = startPos;

        //playerOutline.text = player.text = s;
        //player.color = c;
        //playerOutline.rectTransform.localPosition = startPos;

        //playerOutline.gameObject.SetActive(true);
        inputIcon.gameObject.SetActive(true);
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
            if (JoinMenu.player_count > 2)
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
