using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skipper : MonoBehaviour
{
    public float maxAngle;
    public bool throwing;
    public Rock rock;

    // Start is called before the first frame update
    void Start()
    {
        rock = FindObjectOfType<Rock>();
    }

    // Update is called once per frame
    void Update()
    {
        if (throwing)
            RunThrowLogic();
    }

    private void RunThrowLogic()
    {
        if (InputProxy.p1)
        {
            throwing = false;
            rock.Throw(maxAngle);
        }
    }
}
