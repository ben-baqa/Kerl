using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputIconHUDManager : MonoBehaviour
{
    public int Index
    {
        set { inputIcon.Index = value; }
    }

    public Vector3 rockSelectPosition;
    public Vector3 targetSelectPosition;
    public Vector3 throwPosition;
    public Vector3 brushPosition;

    public float positionLerp = .05f;
    public float sizeDelta = 0.2f, sizeFreq = 1;

    InputIcon inputIcon;

    Vector3 targetPosition;
    float size = 1;

    void Start()
    {
        inputIcon = GetComponentInChildren<InputIcon>();
        targetPosition = rockSelectPosition;
        transform.localPosition = targetPosition;
        inputIcon.Deactivate();
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPosition, positionLerp);

        size = 1 + Mathf.Pow(Mathf.Abs(Mathf.Sin(Time.time * sizeFreq)), .2f) * sizeDelta;
        transform.localScale = Vector3.one * size;
    }

    // TODO: add team colour somewhere
    public void OnTurnStart(bool blueTurn)
    {
        inputIcon.Activate();

        targetPosition = rockSelectPosition;
        transform.localPosition = targetPosition;
    }
    public void OnRockSelected() => targetPosition = targetSelectPosition;
    public void OnTargetSelected() => targetPosition = throwPosition;
    public void OnThrow() => targetPosition = brushPosition;
    public void OnResult() => inputIcon.Deactivate();
}
