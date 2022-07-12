using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class TestInput : MonoBehaviour
{
    public bool StraightInput = false;
    public float Up = 2f;
    public float Down = 1f;
    private static TestInput instance;
    private float value = 0.0f;
    private bool signal;

    private void Start()
    {
        if (instance) {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        signal = false;
    }

    private void Update()
    {
        signal = false;
        if (Keyboard.current["space"].IsPressed())
        {
            value += Up * Time.deltaTime;
            signal = StraightInput || value >= 1;
        }
        else {
            value -= Down * Time.deltaTime;
        }
        value = Mathf.Clamp01(value);
    }

    public static TestInput GetInstance()
    {
        return instance;
    }

    public float GetValue()
    {
        return value;
    }

    public bool GetSignal() {
        return signal;
    }
}