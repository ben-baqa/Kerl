using UnityEngine;
using UnityEngine.Events;

public class NewRock : MonoBehaviour
{
    public float intendedTime = 8;
    public float movementThreshold = 1;

    private float brushingTimer;

    private float movementSpeed;
    private float rotationSpeed;

    private float decay;

    private float rotationThreshold;

    private float simulatedRotation;

    private float notBrushingRatio;

    private Vector3 velocity;

    private Vector3 startPoint;
    private Vector3 midPoint;
    private Vector3 endPoint;

    private bool isThrown = false;
    private bool isBrushing = false;
    private bool isFollowingCurve = false;

    private UnityEvent onDoneBrushing;

    public bool IsBrushing {
        get {
            return isBrushing;
        }
    }

    public UnityEvent OnDoneBrushing {
        get {
            return onDoneBrushing;
        }
    }

    private void FixedUpdate()
    {
        if (isThrown) {
            if (isFollowingCurve) {
                velocity = GetDirection(simulatedRotation) * movementSpeed;
            }

            GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime);
            simulatedRotation += rotationSpeed * Time.deltaTime;

            if (!isBrushing)
            {
                movementSpeed *= Mathf.Pow(decay, Time.deltaTime);
                rotationSpeed *= Mathf.Pow(decay, Time.deltaTime);
            }
            else {
                if (brushingTimer > 0)
                {
                    brushingTimer -= Time.deltaTime;
                }
                else {
                    onDoneBrushing.Invoke();
                }
            }

            if (movementSpeed <= movementThreshold) {
                movementSpeed = 0;
            }

            if (rotationSpeed <= rotationThreshold) {
                rotationSpeed = 0;
            }
        }
    }

    public void Throw(Vector3 p0, Vector3 p1, Vector3 p2, float radius)
    {
        startPoint = p0;
        midPoint = p1;
        endPoint = p2;

        notBrushingRatio = 3 * radius / Vector3.Distance(p0, p2);

        rotationSpeed = (1 - notBrushingRatio) / intendedTime;
        movementSpeed = rotationSpeed * MakeShiftBezierArcLength(100);
        brushingTimer = intendedTime;
        simulatedRotation = 0;

        isThrown = true;
        isBrushing = true;
        isFollowingCurve = true;

        onDoneBrushing = new UnityEvent();
    }

    public void StopBrushing(float brushingProgress) {
        float remainingDistance = 4 * brushingProgress * notBrushingRatio * MakeShiftBezierArcLength(100) / 3;

        decay = Mathf.Exp((movementSpeed / remainingDistance) * (movementThreshold / movementSpeed - 1));

        isBrushing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.GetComponent<NewRock>() != null)
        {
            // TODO: add collision physics
        }
    }

    private Vector3 GetDirection(float rotation)
    {
        return (Vector3.Lerp(midPoint, endPoint, rotation) - Vector3.Lerp(startPoint, midPoint, rotation)).normalized;
    }

    public Vector3[] MakeShiftBezierPoints(int points, float beginning, float amount) {
        Vector3[] result = new Vector3[points];
        for (int i = 0; i < points; i++) {
            result[i] = Bezier.GetPoint(Vector3.zero, midPoint, endPoint, beginning + (float)i / (float)(points - 1) * amount);
        }
        return result;
    }

    public Vector3[] MakeShiftBezierPoints(int points, float amount)
    {
        Vector3[] result = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            result[i] = Bezier.GetPoint(Vector3.zero, midPoint, endPoint, (float)i / (float)(points - 1) * amount);
        }
        return result;
    }

    private float MakeShiftBezierArcLength(int steps)
    {
        Vector3 lastPoint = Vector3.zero;
        float result = 0;
        for (int i = 1; i <= steps; i++)
        {
            Vector3 temp = Bezier.GetPoint(Vector3.zero, midPoint, endPoint, (float)i / (float)steps);
            result += (temp - lastPoint).magnitude;
            lastPoint = temp;
        }
        return result;
    }
}
