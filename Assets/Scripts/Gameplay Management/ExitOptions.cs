using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ExitOptions : MonoBehaviour
{
    [Header("Loading Targets")]
    public string menuScene = "Main Menu";
    public string reloadScene = "Worm Map";

    [Header("Fill Settings")]
    public float drainSpeed;
    public float fillSpeed;
    public float fillMultiplierMax = 3;
    public float fillMultiplierLerp = 0.05f;

    [Header("Input Indicator Settings")]
    public SelectionTokenSettings tokenSettings;

    [Header("Transition Settings")]
    public float startingY;
    public AnimationCurve transitionCurve;
    public float transitionDuration;

    [Header("references")]
    public GameObject readyPopup;

    RectTransform rect;
    ProgressBar progressBar;
    SelectionToken[] inputTokens;

    float progress = 0.5f;
    float fillMultiplier = 1;

    float transitionTimer = 0;

    enum State { Inactive, AwaitingInput, Active};
    State state = State.Inactive;

    private void Start()
    {
        rect = transform as RectTransform;
        rect.anchoredPosition = Vector3.up * startingY;

        readyPopup.SetActive(false);
    }

    void FixedUpdate()
    {
        if (state == State.Inactive)
            return;

        if(state == State.AwaitingInput)
        {
            if (InputProxy.any)
            {
                state = State.Active;
                foreach (SelectionToken i in inputTokens)
                    i.Enable(Vector3.Lerp(tokenSettings.selectedPosition, tokenSettings.neutralPosition, 0.5f));
            }
            return;
        }

        if (transitionTimer < transitionDuration)
        {
            transitionTimer += Time.deltaTime;
            rect.anchoredPosition = Vector3.up * (Mathf.LerpUnclamped(startingY, 0,
                transitionCurve.Evaluate(transitionTimer / transitionDuration)));
        }

        int activeInputs = 0;
        for (int i = 0; i < InputProxy.playerCount; i++)
            if (InputProxy.P(i))
                activeInputs++;

        if (activeInputs > 0)
        {
            if (fillMultiplier < 1)
                fillMultiplier = 1;
            progress += activeInputs * fillMultiplier * fillSpeed / 1000;

            fillMultiplier = Mathf.Lerp(fillMultiplier, fillMultiplierMax, fillMultiplierLerp);
        }
        else
        {
            if (fillMultiplier > 1)
                fillMultiplier = 1;

            progress -= drainSpeed / (1000 * fillMultiplier);

            fillMultiplier = Mathf.Lerp(fillMultiplier, 1 / fillMultiplierMax, fillMultiplierLerp);
        }
        progressBar.SetProgress(progress);

        PlaceIndicators(activeInputs);

        if (progress >= 1)
            SceneManager.LoadScene(reloadScene);
        else if (progress <= 0)
            SceneManager.LoadScene(menuScene);
    }

    public void Activate()
    {
        state = State.AwaitingInput;

        readyPopup.SetActive(true);

        rect.anchoredPosition = Vector3.up * startingY;
        transitionTimer = 0;

        progress = 0.5f;
        progressBar = GetComponentInChildren<ProgressBar>(true);
        progressBar.Init();
        progressBar.Activate();

        int playerCount = InputProxy.playerCount;
        inputTokens = new SelectionToken[playerCount];

        for(int i = 0; i < playerCount; i++)
        {
            inputTokens[i] = new SelectionToken(transform, tokenSettings.size);
        }
    }

    void PlaceIndicators(int on)
    {

        Vector2 position = tokenSettings.neutralPosition;
        Vector2 offset = Vector2.right * tokenSettings.spacing;
        int off = inputTokens.Length - on;
        for (int i = 0; i < off; i++) 
        {
            inputTokens[i].target = position;
            inputTokens[i].Sprite = tokenSettings.offSprite;
            position += offset;
        }

        position = tokenSettings.selectedPosition - (on - 1) * offset;
        for (int i = off; i < inputTokens.Length; i++)
        {
            inputTokens[i].target = position;
            inputTokens[i].Sprite = tokenSettings.onSprite;
            position += offset;
        }

        foreach (SelectionToken i in inputTokens)
            i.Process();
    }
}
