using UnityEngine;
using System;

/// <summary>
/// Runs the physics and scoring of a rock
/// </summary>
public class Rock : MonoBehaviour
{
    [Header("Movement")]
    public float intendedTime = 8;
    public float movementThreshold = 1;
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
        || movementSpeed > movementThreshold)
        && rb.position.y > -0.5f;

    float brushingTimer;

    float movementSpeed;
    float rotationSpeed;

    float decay;

    float simulatedRotation;

    float notBrushingRatio;

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

    public bool IsBrushing
    {
        get
        {
            return isBrushing;
        }
    }

    private void Start()
    {
        sfx = GetComponent<AudioSource>();

        //transform.GetChild(1).gameObject.SetActive(isScoring);
    }

    private void FixedUpdate()
    {
        if (isThrown)
        {
            if (isFollowingCurve)
            {
                velocity = GetDirection(simulatedRotation) * movementSpeed;
                simulatedRotation += rotationSpeed * Time.deltaTime;
            }
            else
            {
                velocity = velocity.normalized * movementSpeed;
            }

            if (!rb)
                rb = GetComponent<Rigidbody>();
            rb.MovePosition(transform.position + velocity * Time.deltaTime);


            if (!isBrushing)
            {
                movementSpeed *= Mathf.Pow(decay, Time.deltaTime);
                rotationSpeed *= Mathf.Pow(decay, Time.deltaTime);
            }
            else
            {
                if (brushingTimer > 0)
                {
                    brushingTimer -= Time.deltaTime;
                }
                else
                {
                    RoundManager.OnRockPassResultThreshold();
                    //onDoneBrushing.Invoke();
                }
            }

            if (movementSpeed <= movementThreshold)
            {
                movementSpeed = 0;
                rotationSpeed = 0;
                isFollowingCurve = false;
                hitBy = null;

                if (!hasEndedTurn)
                {
                    hasEndedTurn = true;
                    RoundManager.OnRockStop();
                }
            }

            if (rb.position.y < -0.5f && !hasSlipped)
            {
                hasSlipped = true;
                slip.Play();
                if (!hasEndedTurn)
                {
                    RoundManager.OnRockStop();
                    hasEndedTurn = true;
                }
            }

            if (rb.position.y < -fallingKillThreshold)
                Destroy(gameObject);
        }
    }

    public void Throw(Vector3 p0, Vector3 p1, Vector3 p2, float notBrushingRatio)
    {
        startPoint = p0;
        midPoint = p1;
        endPoint = p2;

        this.notBrushingRatio = notBrushingRatio;
        //notBrushingRatio = 3 * radius / Vector3.Distance(p0, p2);
        //print("not brushing ratio: " + notBrushingRatio);

        rotationSpeed = (1 - notBrushingRatio) / intendedTime;
        movementSpeed = rotationSpeed * MakeShiftBezierArcLength(100);
        //movementSpeed = MakeShiftBezierArcLength(100) / intendedTime;
        //print("Bezier length: " + MakeShiftBezierArcLength(100));
        brushingTimer = intendedTime;
        simulatedRotation = 0;

        isThrown = true;
        isBrushing = true;
        isFollowingCurve = true;
    }

    public void Score(bool scoring)
    {
        transform.GetChild(1).gameObject.SetActive(scoring);
        if (scoring && !isScoring)
        {
            onScore.Play();
        }
        else if (isScoring && !scoring)
        {
            onLoseScore.Play();
        }
        isScoring = scoring;
    }

    public void StopBrushing(float brushingProgress)
    {
        float remainingDistance = 4 * brushingProgress * notBrushingRatio * MakeShiftBezierArcLength(100) / 3;

        decay = Mathf.Exp((movementSpeed / remainingDistance) * (movementThreshold / movementSpeed - 1));

        isBrushing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.GetComponent<Rock>() != null)
        {
            if (target.GetComponent<Rock>() == hitBy)
                return;

            isFollowingCurve = false;
            Vector3 toTarget = (target.transform.position - transform.position).normalized;
            Vector3 oldVelocity = velocity;
            velocity -= Vector3.Project(velocity, toTarget);
            movementSpeed = velocity.magnitude;
            target.GetComponent<Rock>().AddRockCollision(this, Vector3.Project(oldVelocity, toTarget), decay);

            if (sfx && !sfx.isPlaying)
            {
                sfx.volume = Mathf.Clamp01(movementSpeed * 3);
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
        movementSpeed = velocity.magnitude;
        this.decay = decay;
        this.hitBy = gotHitBy;
    }

    private Vector3 GetDirection(float rotation)
    {
        return (Vector3.Lerp(midPoint, endPoint, rotation) - Vector3.Lerp(startPoint, midPoint, rotation)).normalized;
    }

    public Vector3[] GetPredictionPoints(int points, float beginning, float amount)
    {
        Vector3[] result = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            result[i] = GetUnclampedBezierPoint(startPoint, midPoint, endPoint, beginning + (float)i / (float)(points - 1) * amount);
        }
        return result;
    }

    private float MakeShiftBezierArcLength(int steps)
    {
        Vector3 lastPoint = startPoint;
        float result = 0;
        for (int i = 1; i <= steps; i++)
        {
            Vector3 temp = GetUnclampedBezierPoint(startPoint, midPoint, endPoint, (float)i / (float)steps);
            result += (temp - lastPoint).magnitude;
            lastPoint = temp;
        }
        return result;
    }

    public static Vector3 GetUnclampedBezierPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Max(0, t);
        float oneMinusT = 1 - t;
        return
            oneMinusT * oneMinusT * p0 +
            2 * oneMinusT * t * p1 +
            t * t * p2;
    }
}
