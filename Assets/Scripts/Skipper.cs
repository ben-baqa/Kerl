using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skipper : MonoBehaviour
{
    public float period = 2, maxAngle, pushDelay = .5f;
    public bool weightedCurve, throwing;
    public int rocks = 5;

    public GameObject blueRock, redRock;
    public Material redMat, blueMat;

    private Animator anim;
    private CurveLine line;
    private SkinnedMeshRenderer rend;
    private Rock rock;
    private TurnManager input;
    private AudioSource sfx;
    private Sweeper sweeper;

    private float n, angle;
    private bool blueTurn = true;
    private int throwCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponentInChildren<Animator>();
        line = GetComponentInChildren<CurveLine>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();
        input = FindObjectOfType<TurnManager>();
        sweeper = FindObjectOfType<Sweeper>();
        sfx = GetComponent<AudioSource>();
        StartTurn(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (throwing)
            RunThrowLogic();
    }

    private void RunThrowLogic()
    {
        angle = Angle();
        line.Generate(angle);
        
        if (input.GetInput())
        {
            throwing = false;
            n = 0;
            line.OnPush();
            input.OnThrow();
            sfx.Play();
            StartCoroutine(Throw());
        }
    }

    private IEnumerator Throw()
    {
        yield return new WaitForSeconds(pushDelay);
        rock.Throw(angle, angle / maxAngle);
        throwCount++;
        anim.SetTrigger("push");
        CameraPositions.OnPush(rock.transform);
        sweeper.OnThrow(rock.transform);
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

    public void StartTurn(bool b = true)
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
            rend.material = blueMat;
            sweeper.OnTurnStart(blueMat);
        }
        else
        {
            rock = Instantiate(redRock).GetComponent<Rock>();
            rock.skip = this;
            rend.material = redMat;
            sweeper.OnTurnStart(redMat);
        }
        blueTurn = !blueTurn;

        if (b)
            input.OnTurn();
    }
}
