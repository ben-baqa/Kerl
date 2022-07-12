using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestNewRock : MonoBehaviour
{
    private Vector3 startPoint;
    private Vector3 midPoint;
    private Vector3 endPoint;

    public float Decay;
    public float Threshold;

    private Vector3 velocity;

    private float movementSpeed;
    private float rotationSpeed;

    private float movementThreshold;
    private float rotationThreshold;

    private float movementDecay = 1;
    private float rotationDecay = 1;

    private float simulatedRotation;
    
    public bool thrown = false;

    private void FixedUpdate()
    {
        if (thrown)
        {
            velocity = GetDirection(simulatedRotation) * movementSpeed;

            GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime);
            simulatedRotation += rotationSpeed * Time.deltaTime;

            movementSpeed *= Mathf.Pow(Decay * movementDecay, Time.deltaTime);
            rotationSpeed *= Mathf.Pow(Decay * rotationDecay, Time.deltaTime);

            if (movementSpeed <= movementThreshold)
            {
                movementSpeed = 0;
                rotationSpeed = 0;
            }

            if (rotationSpeed <= rotationThreshold)
            {
                rotationSpeed = 0;
            }
        }
    }

    private float MakeShiftBezierArclength(int steps) {
        Vector3 lastPoint = Vector3.zero;
        float ret = 0;
        for (int i = 1; i <= steps; i++)
        {
            Vector3 temp = Bezier.GetPoint(Vector3.zero, midPoint, endPoint, (float)i / (float)steps);
            ret += (temp - lastPoint).magnitude;
            lastPoint = temp;
        }
        return ret;
    }

    public Vector3[] MakeShiftPrediction(int steps)
    {
        Vector3[] ret = new Vector3[steps + 1];
        float delta = Mathf.Log((movementThreshold) / movementSpeed, Decay) / (float) (steps);
        if (delta > 0)
        {
            float tempMD = Mathf.Pow(Decay * movementDecay, delta);
            float tempRD = Mathf.Pow(Decay * rotationDecay, delta);

            Vector3 tempP = transform.position;
            float tempSR = simulatedRotation;

            float tempMS = movementSpeed;
            float tempRS = rotationSpeed;

            for (int i = 0; i <= steps; i++)
            {
                ret[i] = tempP;
                tempP += GetDirection(tempSR) * tempMS * delta;
                tempSR += tempRS * delta;

                tempMS *= tempMD;
                tempRS *= tempRD;
            }
        }
        return ret;
    }

    public void Throw(Vector3 p0, Vector3 p1, Vector3 p2) {
        startPoint = p0;
        midPoint = p1;
        endPoint = p2;

        transform.localPosition = startPoint;

        movementThreshold = Threshold;
        rotationThreshold = Threshold / MakeShiftBezierArclength(20);

        movementSpeed = movementThreshold - MakeShiftBezierArclength(20) * Mathf.Log(Decay);
        rotationSpeed = rotationThreshold - Mathf.Log(Decay);
        simulatedRotation = 0;
        thrown = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.GetComponent<TestNewRock>() != null) {
            velocity *= 0f;
        }
    }

    private Vector3 GetDirection(float rotation) {
        return (Vector3.Lerp(midPoint, endPoint, rotation) - Vector3.Lerp(startPoint, midPoint, rotation)).normalized;
    }

    public void Distort(float distortion) {
        movementDecay = distortion;
        //rotationDecay = 1f / distortion;
    }
}
