using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Runs the brushing logic
/// </summary>
public class Sweeper : MonoBehaviour
{
    enum Follow { rock, result, start }
    Follow followState = Follow.start;


    [Header("Sweeping Physics")]
    public float progressUpRate = 0.1f;
    public float progressDownRate = 0.05f;
    public float progressRateLerp = 0.05f;

    [Header("Rock Movement Settings")]
    public float defaultBrushingTime = 6;
    public float BrushingTime
    {
        get => _brushingTime;
        set
        {
            _brushingTime = value;
            if (rock)
                rock.brushingTime = value;
        }
    }
    float _brushingTime;

    public AnimationCurve brushingMovementCurve;
    public float resultSpeed = 5;

    [Header("body movement")]
    public float lerpLerp, normalLerp;
    float followLerp;

    Character character;

    TurnManager turnManager;
    CurlingBar curlingBar;
    Rock rock;

    Vector3 startPoint;
    Vector3 anchor;
    Vector3 upLeft;
    Vector3 downRight;

    float curveZDistance;
    float targetRadius;
    float brushingRatio;
    float brushingProgress;
    float progressRate;

    void Awake()
    {
        turnManager = FindObjectOfType<TurnManager>();

        curlingBar = FindObjectOfType<CurlingBar>();
        curlingBar.gameObject.SetActive(false);

        BrushingTime = defaultBrushingTime;
    }

    private void FixedUpdate()
    {
        switch (followState)
        {
            case Follow.rock:
                character.MoveToRock(rock.position, followLerp);
                followLerp = Mathf.Lerp(followLerp, 1, lerpLerp);

                if (followLerp > .3f)
                {
                    if (turnManager.GetInput())
                    {
                        if (progressRate < 0)
                            progressRate = 0;

                        progressRate = Mathf.Lerp(progressRate, progressUpRate, progressRateLerp * Time.deltaTime);
                        //brushingProgress += progressUpRate ;
                        character.BrushSpeed = progressRate / progressUpRate;
                    }
                    else
                    {
                        if (progressRate > 0)
                            progressRate = 0;

                        progressRate = Mathf.Lerp(progressRate, -progressDownRate, progressRateLerp * Time.deltaTime);
                        //brushingProgress -= progressDownRate * 0.02f / BrushingTime;
                        character.BrushSpeed = 0;
                    }
                }
                brushingProgress += progressRate * 0.02f / BrushingTime;
                brushingProgress = Mathf.Clamp(brushingProgress, .25f, 1);

                UpdateCurlingbar();
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
        progressRate = 0;
    }

    public void SetRock(Rock r)
    {
        rock = r;
        //rock.Init(BrushingTime, brushingMovementCurve);
        rock.brushingTime = BrushingTime;
        rock.brushingMovementCurve = brushingMovementCurve;
    }

    public void OnTurnStart()
    {
        followState = Follow.start;
    }

    public void SetCharacter(Character c)
    {
        character = c;
    }

    public void OnThrow(Vector3 startPoint, Vector3 throwingDirection, float targetRadius, float brushingRatio)
    {
        curlingBar.gameObject.SetActive(true);
        followState = Follow.rock;
        followLerp = 0;

        this.startPoint = startPoint;
        this.targetRadius = targetRadius;
        curveZDistance = throwingDirection.z - startPoint.z;
        this.brushingRatio = brushingRatio;
        brushingProgress = .75f;

        anchor = throwingDirection - ((Quaternion.Euler(0, 90, 0) * throwingDirection).normalized + throwingDirection.normalized * 3) * targetRadius;
        upLeft = throwingDirection.normalized * targetRadius * 4;
        downRight = (Quaternion.Euler(0, 90, 0) * throwingDirection).normalized * targetRadius * 2;
    }

    public void OnResult()
    {
        followState = Follow.result;
        followLerp = normalLerp;
        character.OnResult();

        rock.StopBrushing(brushingProgress, resultSpeed);
        curlingBar.gameObject.SetActive(false);
    }

    private void UpdateCurlingbar()
    {
        //curlingBar.UpdateProgress(brushingProgress);
        curlingBar.Progress = brushingProgress;

        Vector3[] currentPredictionPoints = GetPredictionPoints();
        List<Vector2> convertedPredictionPoints = new List<Vector2>();

        for (int i = 0; i < currentPredictionPoints.Length; i++)
        {
            convertedPredictionPoints.Add(ConvertToUI(currentPredictionPoints[i]));
        }

        curlingBar.UpdatePredictionLine(convertedPredictionPoints);
    }

    private Vector2 ConvertToUI(Vector3 original)
    {
        Vector3 translated = original - anchor - startPoint;
        Vector3 downRightProjection = Vector3.Project(translated, downRight);
        Vector3 upLeftProjection = Vector3.Project(translated, upLeft);
        return new Vector2(downRightProjection.magnitude / downRight.magnitude * (IsPositive(downRightProjection, downRight) ? 1 : -1), upLeftProjection.magnitude / upLeft.magnitude * (IsPositive(upLeftProjection, upLeft) ? 1 : -1));
    }

    private bool IsPositive(Vector3 projection, Vector3 axis)
    {
        return projection.x * axis.x >= 0 && projection.y * axis.y >= 0 && projection.z * axis.z >= 0;
    }

    public Vector3[] GetPredictionPoints()
    {
        //return rock.GetPredictionPoints(20, brushingRatio, 4 * (1 - brushingRatio) * brushingProgress / 3);

        return rock.GetPredictionPoints(20, brushingRatio, 1 + (targetRadius * (brushingProgress * 4 - 3)) / curveZDistance);
        
    }
}
