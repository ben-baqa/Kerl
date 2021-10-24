using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skipper : MonoBehaviour
{
    public float period = 2, maxAngle;
    public bool weightedCurve, throwing;
    public int rocks = 5;

    public GameObject blueRock, redRock;

    private Animator anim;
    private CurveLine line;
    private Rock rock;

    private float n;
    private bool blueTurn = true;
    private int throwCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        line = GetComponentInChildren<CurveLine>();
        StartTurn();
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
            rock.Throw(angle, angle / maxAngle);
            throwCount++;
            anim.SetTrigger("push");
            CameraPositions.OnPush(rock.transform);
            line.Hide();
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

    public void StartTurn()
    {
        if (throwCount > rocks * 2)
            return;
        // end of game

        CameraPositions.OnTurnStart();
        throwing = true;
        if (blueTurn)
        {
            rock = Instantiate(blueRock).GetComponent<Rock>();
            rock.skip = this;
        }
        else
        {
            rock = Instantiate(redRock).GetComponent<Rock>();
            rock.skip = this;
        }
        blueTurn = !blueTurn;
    }
}
