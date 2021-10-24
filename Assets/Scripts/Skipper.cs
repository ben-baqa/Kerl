using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skipper : MonoBehaviour
{
    public float period = 2, maxAngle;
    public bool weightedCurve, throwing;

    private Rock rock;
    private CurveLine line;

    private float n;

    // Start is called before the first frame update
    void Start()
    {
        rock = FindObjectOfType<Rock>();
        line = GetComponentInChildren<CurveLine>();
    }

    // Update is called once per frame
    void Update()
    {
        if (throwing)
            RunThrowLogic();
    }

    private void RunThrowLogic()
    {
        float angle = Angle();
        line.Generate(angle);
        
        if (InputProxy.p1)
        {
            throwing = false;
            n = 0;
            rock.Throw(angle);
        }
    }

    private float Angle()
    {
        n += Time.deltaTime;
        if (n > period)
            n -= period;

        float sin = Mathf.Sin((n / period) * 2 * Mathf.PI);
        if (weightedCurve)
            return maxAngle * Mathf.Pow(sin, 3);
        return maxAngle * sin;
    }
}
