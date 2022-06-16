using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EmotivUnityPlugin;
using UnityEngine.UI;
using TMPro;

public class ErrorDisplay : MonoBehaviour
{
    public float duration = 5;

    TextMeshProUGUI text;
    CanvasGroup canvasGroup;

    float timer = 0;

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(gameObject);
        Cortex.ErrorRecieved += OnErrorMessageRecieved;

        text = GetComponentInChildren<TextMeshProUGUI>(true);
        canvasGroup = GetComponentInChildren<CanvasGroup>(true);
        canvasGroup.alpha = 0;
    }

    void OnErrorMessageRecieved(ErrorMsgEventArgs args)
    {
        timer = duration;
        text.text = $"Error: {args.MessageError} on method {args.MethodName}";
        canvasGroup.alpha = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
            canvasGroup.alpha = Mathf.Lerp(canvasGroup.alpha, 0, Time.deltaTime);
    }
}
