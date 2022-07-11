using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Runs the brushing logic
/// </summary>
public class FakeSweeper : MonoBehaviour
{
    public Vector3 offsetFromRock, resultPosition;

    [Header("Sweeping Physics")]
    public float frictionMultipler = 1;
    public float decay = .01f, sweepValue = .1f;

    [Header("body movement")]
    public float followLerp;
    public float lerpLerp, normalLerp, rotLerp,
        brushRot, startRot, resultRot;


    Character character;

    TurnManager input;
    CurlingBar barDisplay;
    Rock rock;

    Vector3 startPosition;

    enum Follow { rock, result, start }
    Follow followState = Follow.start;


    void Awake()
    {
        input = FindObjectOfType<TurnManager>();

        barDisplay = FindObjectOfType<CurlingBar>();

        startPosition = transform.position;
    }

    void FixedUpdate()
    {
        frictionMultipler += decay / 50;
        bool sliding = followState == Follow.rock;
        barDisplay.gameObject.SetActive(sliding);

        switch (followState)
        {
            case Follow.rock:
                character.MoveToRock(rock.position, followLerp);
                followLerp = Mathf.Lerp(followLerp, 1, lerpLerp);

                PredictLand();

                if (followLerp > .3f)
                {
                    if (input.GetInput())
                    {
                        frictionMultipler -= sweepValue;
                        character.BrushSpeed = 1;
                    }
                    else
                        character.BrushSpeed = 0;
                }
                break;
            case Follow.result:
                character.MoveToResult(followLerp);
                break;
        }
    }

    public void OnThrow()
    {
        followState = Follow.rock;
        followLerp = 0;
        frictionMultipler = 1;
    }

    public void SetRock(Rock r)
    {
        rock = r;
    }

    public void OnResult()
    {
        followState = Follow.result;
        followLerp = normalLerp;
        character.OnResult();
    }

    public void OnTurnStart()
    {
        followState = Follow.start;
    }

    public void SetCharacter(Character c)
    {
        character = c;
        offsetFromRock = c.brushingPlacement.position;
        brushRot = c.brushingPlacement.rotation;

        resultPosition = c.resultPlacement.position;
        resultRot = c.resultPlacement.rotation;
    }

    // predict landing zone, 1 = 82, .75 = 75, 0 = 54
    public void PredictLand()
    {
        Rigidbody rb = rock.GetComponent<Rigidbody>();

        rock.frictionMultiplier = frictionMultipler;

        Vector3 pos = rb.position;
        Vector3 vel = rb.velocity;
        float radVel = rb.angularVelocity.y,
            friction = rock.friction * frictionMultipler;

        int safetyCount = 0;

        while (vel.magnitude > rock.stopThreshold)
        {
            pos += vel / 50;
            vel += Vector3.right * radVel * rock.spinForce / 50;

            vel *= (1 - friction);
            radVel -= radVel * rb.angularDrag;

            if (vel.magnitude < rock.slowDownThreshold)
            {
                vel = Vector3.Lerp(vel, Vector3.zero, rock.slowDownLerp);
            }

            if (safetyCount++ > 1000)
            {
                print("Yipes! " + safetyCount + " was not enough!");
                break;
            }
        }

        //string s = "Long: " + pos.z + ", Calc: ";
        //pos.z = (rb.velocity.z * (1 - friction) / -50) / Mathf.Log(1 - friction);
        //print(s + pos.z);

        // 21 = .75, 19.5 = 0.5
        float v = Mathf.Clamp01((pos.z - 16.5f) / 6);
        barDisplay.progress = v;
    }
}
