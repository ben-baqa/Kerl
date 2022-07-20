using UnityEngine;
using System;

/// <summary>
/// Runs the physics and scoring of a rock
/// </summary>
public class Rock : MonoBehaviour
{

    [Header("Movement")]
    public float movementThreshold = 1;
    public float slipThreshold = 0.1f;
    [Min(10)]
    public float fallingKillThreshold = 100;

    [Header("Sounds")]
    public AudioClip[] collisionSounds;
    public AudioSource slip;
    public AudioSource onScore;
    public AudioSource onLoseScore;

    [Space(20)]
    public bool blue;

    public Vector3 position => transform.position;
    public bool IsMoving =>
        (rb.velocity.magnitude > movementThreshold
        || isFollowingCurve) && !hasSlipped;

    AnimationCurve brushingMovementCurve;
    float brushingTime = 8;
    float brushTimer;
    float brushingRatio;

    float resultThresholdToTarget;
    float targetRadius;

    float progress;
    float progressSpeed;
    float decay;

    Vector3 velocity;

    Vector3 startPoint;
    Vector3 midPoint;
    Vector3 endPoint;

    bool isThrown = false;
    bool isBrushing = false;
    bool isFollowingCurve = false;

    bool hasEndedTurn = false;
    bool isScoring = false;
    bool hasSlipped = false;

    Rigidbody rb;
    AudioSource sfx;

    Rock hitBy;

    private void Start()
    {
        sfx = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
    }

    public void Init(float travelTime, AnimationCurve movementCurve)
    {
        brushingTime = travelTime;
        brushingMovementCurve = movementCurve;
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            if (isFollowingCurve)
            {
                if (isBrushing)
                {
                    float brushingProgress = brushTimer / brushingTime;
                    brushingProgress = brushingMovementCurve.Evaluate(brushingProgress);

                    Vector3 newPosition = GetUnclampedBezierPoint(brushingRatio * brushingProgress);
                    velocity = (newPosition - position) / Time.deltaTime;

                    rb.MovePosition(newPosition);

                    brushTimer += Time.deltaTime;
                    if (brushTimer >= brushingTime)
                    {
                        RoundManager.OnRockPassResultThreshold();
                        return;
                    }
                }
                else
                {
                    Vector3 newPosition = GetUnclampedBezierPoint(progress);
                    velocity = (newPosition - position) / Time.deltaTime;

                    rb.MovePosition(newPosition);
                    progress += progressSpeed * Time.deltaTime;

                    float scaledDecay = Mathf.Pow(decay, Time.deltaTime);
                    progressSpeed *= scaledDecay;
                }
            }
            else
            {
                rb.velocity *= Mathf.Pow(decay, Time.deltaTime);
                velocity = rb.velocity;
            }

            if (!isBrushing)
            {
                if (velocity.magnitude <= movementThreshold)
                {
                    rb.velocity = Vector3.zero;
                    velocity = Vector3.zero;
                    isFollowingCurve = false;
                    hitBy = null;

                    if (!hasEndedTurn)
                    {
                        hasEndedTurn = true;
                        RoundManager.OnRockStop();
                    }
                }
            }

            

            if (position.y < -slipThreshold && !hasSlipped)
            {
                hasSlipped = true;
                isFollowingCurve = false;
                slip.Play();
                if (!hasEndedTurn)
                {
                    hasEndedTurn = true;
                    RoundManager.OnRockStop();
                }
            }

            if (position.y < -fallingKillThreshold)
                Destroy(gameObject);
        }
    }

    public void Throw(Vector3 start, Vector3 mid, Vector3 end, float brushingRatio, float targetRadius)
    {
        startPoint = start;
        midPoint = mid;
        endPoint = end;

        isThrown = true;
        isBrushing = true;
        isFollowingCurve = true;

        brushTimer = 0;
        this.brushingRatio = brushingRatio;
        this.targetRadius = targetRadius;

        resultThresholdToTarget = (1 - brushingRatio) * (end.z - start.z);
    }

    public void Score(bool scoring)
    {
        if (scoring && !isScoring)
            onScore.Play();
        else if (isScoring && !scoring)
            onLoseScore.Play();

        transform.GetChild(1).gameObject.SetActive(scoring);
        isScoring = scoring;
    }

    /// <summary>
    /// exit the brushing input state to glide into final position determined by the brushing progress
    /// </summary>
    /// <param name="brushingProgress">0.75 is on target, 1 is target + radius</param>
    public void StopBrushing(float brushingProgress, float initialSpeed)
    {
        isBrushing = false;
        // result movement is done with simple exponential decay,
        // aiming to land at the exact point determined by brushingProgress
        // using only z distance / speed as a proxy
        // v(t) = vi * (decay)^t
        // d(t) = di + v(t) / ln(decay)
        velocity = Vector3.forward * initialSpeed;

        float extraDistance = targetRadius * (brushingProgress * 4 - 3);
        float remainingDistance = resultThresholdToTarget + extraDistance;

        // convert initial physical speed to curve interpolation space
        progressSpeed = initialSpeed / (endPoint.z - startPoint.z);
        progress = brushingRatio + progressSpeed * Time.deltaTime;

        // solving for decay using distance when v(t) == movementThreshold
        decay = Mathf.Exp((movementThreshold - initialSpeed) / remainingDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject collisionObject = collision.gameObject;
        Rock collisionRock = collisionObject.GetComponent<Rock>();
        if (collisionRock != null && collisionRock != hitBy)
        {
            isBrushing = false;
            isFollowingCurve = false;
            collisionRock.isFollowingCurve = false;

            Vector3 toTarget = (collisionRock.position - position).normalized;
            Vector3 oldVelocity = velocity;
            velocity -= Vector3.Project(velocity, toTarget);
            rb.velocity = velocity;

            collisionRock.AddRockCollision(this, Vector3.Project(oldVelocity, toTarget), decay);

            if (sfx && !sfx.isPlaying)
            {
                sfx.volume = Mathf.Clamp01(rb.velocity.magnitude * 3);
                sfx.clip = collisionSounds[UnityEngine.Random.Range(0, collisionSounds.Length)];
                sfx.Play();
            }
        }
    }

    public void AddRockCollision(Rock gotHitBy, Vector3 velocity, float decay)
    {
        isThrown = true;
        isFollowingCurve = false;
        this.velocity = velocity;
        this.decay = decay;
        hitBy = gotHitBy;
        rb.velocity = velocity;
    }

    private Vector3 GetUnclampedBezierPoint(float t)
    {
        return GetUnclampedBezierPoint(startPoint, midPoint, endPoint, t);
    }

    private Vector3 GetUnclampedBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Max(0, t);
        float oneMinusT = 1 - t;
        return
            oneMinusT * oneMinusT * p0 +
            2 * oneMinusT * t * p1 +
            t * t * p2;
    }

    public Vector3[] GetPredictionPoints(int steps, float start, float distance)
    {
        Vector3[] result = new Vector3[steps];
        for (int i = 0; i < steps; i++)
        {
            result[i] = GetUnclampedBezierPoint(start + (float)i / (float)(steps - 1) * distance);
        }
        return result;
    }

    //private Vector3 GetDirection(float t)
    //{
    //    return (Vector3.Lerp(midPoint, endPoint, t) - Vector3.Lerp(startPoint, midPoint, t)).normalized;
    //}

    //private float MakeShiftBezierArcLength(int steps)
    //{
    //    Vector3 lastPoint = startPoint;
    //    float result = 0;
    //    for (int i = 1; i <= steps; i++)
    //    {
    //        Vector3 temp = GetUnclampedBezierPoint((float)i / (float)steps);
    //        result += (temp - lastPoint).magnitude;
    //        lastPoint = temp;
    //    }
    //    return result;
    //}

    //private float GetExtendedBezierLength(int steps, float extension)
    //{
    //    Vector3 lastPoint = startPoint;
    //    float result = 0;
    //    for (int i = 1; i <= steps; i++)
    //    {
    //        Vector3 temp = GetUnclampedBezierPoint(i * extension / steps);
    //        result += (temp - lastPoint).magnitude;
    //        lastPoint = temp;
    //    }
    //    return result;
    //}
}
