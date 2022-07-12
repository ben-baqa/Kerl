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
    public Vector3 inputOffPosition;
    public Vector3 inputOnPosition;
    public float inputIndicatorOffset;
    public float indicatorSize;

    public Sprite inputOnIndicatorSprite;
    public Sprite inputOffIndicatorSprite;

    [Header("Transition Settings")]
    public float startingY;
    public AnimationCurve transitionCurve;
    public float transitionDuration;

    RectTransform rect;
    ProgressBar progressBar;
    InputIndicator[] inputIndicators;

    float progress = 0.5f;
    float fillMultiplier = 1;

    float transitionTimer = 0;

    enum State { Inactive, AwaitingInput, Active};
    State state = State.Inactive;

    private void Start()
    {
        rect = transform as RectTransform;
        rect.anchoredPosition = Vector3.up * startingY;
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
                foreach (InputIndicator i in inputIndicators)
                    i.Enable(Vector3.Lerp(inputOnPosition, inputOffPosition, 0.5f));
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

        rect.anchoredPosition = Vector3.up * startingY;
        transitionTimer = 0;

        progress = 0.5f;
        progressBar = GetComponentInChildren<ProgressBar>(true);
        progressBar.Init();
        progressBar.Activate();

        int playerCount = InputProxy.playerCount;
        inputIndicators = new InputIndicator[playerCount];

        for(int i = 0; i < playerCount; i++)
        {
            inputIndicators[i] = new InputIndicator(transform, indicatorSize);
        }
    }

    void PlaceIndicators(int on)
    {

        Vector3 position = inputOffPosition;
        Vector3 offset = Vector3.right * inputIndicatorOffset;
        int off = inputIndicators.Length - on;
        for (int i = 0; i < off; i++) 
        {
            inputIndicators[i].target = position;
            inputIndicators[i].Sprite = inputOffIndicatorSprite;
            position += offset;
        }

        position = inputOnPosition - (on - 1) * offset;
        for (int i = off; i < inputIndicators.Length; i++)
        {
            inputIndicators[i].target = position;
            inputIndicators[i].Sprite = inputOnIndicatorSprite;
            position += offset;
        }

        foreach (InputIndicator i in inputIndicators)
            i.Process();
    }

    class InputIndicator
    {
        public Vector3 target;
        public Sprite Sprite { set { image.sprite = value; } }

        RectTransform rect;
        Image image;

        public InputIndicator(Transform parent, float size)
        {
            GameObject indicatorInstance = new GameObject("indicator");
            indicatorInstance.transform.parent = parent;
            rect = indicatorInstance.AddComponent<RectTransform>();
            rect.sizeDelta = Vector2.one * size;
            rect.localScale = Vector3.one;
            image = indicatorInstance.AddComponent<Image>();
            image.enabled = false;
        }

        public void Enable(Vector3 position)
        {
            image.enabled = true;
            rect.anchoredPosition = position;
        }

        public void Process()
        {
            rect.anchoredPosition = Vector3.Lerp(rect.anchoredPosition, target, 0.2f);
        }
    }
}
