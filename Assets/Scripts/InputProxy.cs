using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProxy : MonoBehaviour
{
    public bool p1 { get { return bcip1 || nump1; } }
    public bool p2 { get { return bcip2 || nump2; } }
    public bool p3 { get { return bcip3 || nump3; } }
    public bool p4 { get { return bcip4 || nump4; } }

    public bool enableNumpadInput = true;

    private bool bcip1, bcip2, bcip3, bcip4,
        nump1, nump2, nump3, nump4;


    private void Update()
    {
        nump1 = Input.GetKey(KeyCode.Keypad1);
        nump2 = Input.GetKey(KeyCode.Keypad2);
        nump3 = Input.GetKey(KeyCode.Keypad3);
        nump4 = Input.GetKey(KeyCode.Keypad4);
    }

    public void OnInput(bool P1)
    {
        bcip1 = P1;
        bcip2 = false;
        bcip3 = false;
        bcip4 = false;
    }

    public void SetP1(bool b) { bcip1 = b; }
    public void SetP2(bool b) { bcip2 = b; }
    public void SetP3(bool b) { bcip3 = b; }
    public void SetP4(bool b) { bcip4 = b; }
}
