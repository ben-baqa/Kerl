using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProxy : MonoBehaviour
{
    public bool p1, p2, p3, p4;
    public bool keyboardDebug;

    private void Start()
    {
//#if !UNITY_EDITOR
//        keyboardDebug = false;
//#endif

    }

    private void Update()
    {
        if (keyboardDebug)
        {
            p1 |= Input.GetKey(KeyCode.Keypad1);
            p2 |= Input.GetKey(KeyCode.Keypad2);
            p3 |= Input.GetKey(KeyCode.Keypad3);
            p4 |= Input.GetKey(KeyCode.Keypad4);
        }
    }

    public void OnInput(bool P1)
    {
        if (keyboardDebug)
            return;
        p1 = P1;
        p2 = false;
        p3 = false;
        p4 = false;
    }
}
