using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProxy : MonoBehaviour
{
    public bool p1 { get { return bcip1 || keyp1; } }
    public bool p2 { get { return bcip2 || keyp2; } }
    public bool p3 { get { return bcip3 || keyp3; } }
    public bool p4 { get { return bcip4 || keyp4; } }

    public bool enableNumpadInput = true;

    private bool bcip1, bcip2, bcip3, bcip4,
        keyp1, keyp2, keyp3, keyp4;


    private void Update()
    {
        keyp1 = Input.GetKey(KeyCode.Keypad1) || Input.GetKey(KeyCode.A);
        keyp2 = Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.F);
        keyp3 = Input.GetKey(KeyCode.Keypad3) || Input.GetKey(KeyCode.J);
        keyp4 = Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.Semicolon);
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
