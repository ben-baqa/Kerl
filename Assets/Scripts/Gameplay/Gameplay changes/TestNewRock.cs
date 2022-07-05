using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TestNewRock : MonoBehaviour
{
    public Vector3 MidPoint;
    public Vector3 EndPoint;

    public float Decay;

    public float Threshold;

    private Vector3 velocity;
    private float traveled = 0;
    private bool inOrbit = true;

    private void Start()
    {
        velocity = MidPoint.normalized * (Threshold - MakeShiftBezierArclength(100) * Mathf.Log(Decay));
    }

    private void FixedUpdate()
    {
        traveled += velocity.magnitude * Time.deltaTime;
        GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime);
        velocity *= Mathf.Pow(Decay, Time.deltaTime);
        if (inOrbit)
        { 
            Vector3 destination = Vector3.Lerp(MidPoint, EndPoint, traveled / MakeShiftBezierArclength(100));
            velocity = (destination - transform.localPosition).normalized * velocity.magnitude;
        }

        if (velocity.magnitude <= Threshold) {
            velocity = Vector3.zero;
        }
    }

    private float MakeShiftBezierArclength(int steps) {
        Vector3 lastPoint = Vector3.zero;
        float ret = 0;
        for (int i = 1; i <= steps; i++)
        {
            Vector3 temp = Bezier.GetPoint(Vector3.zero, MidPoint, EndPoint, (float)i / (float)steps);
            ret += (temp - lastPoint).magnitude;
            lastPoint = temp;
        }
        return ret;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject target = collision.gameObject;
        if (target.GetComponent<TestNewRock>() != null) {
            velocity *= -5.0f;
            inOrbit = false;
        }
    }
}

[CustomEditor(typeof(TestNewRock))]
public class TestNewRockEditor : Editor
{
    private TestNewRock rock;
    private Transform handleTransform;

    private void OnSceneGUI()
    {
        rock = target as TestNewRock;
        handleTransform = rock.transform;

        Vector3 p0 = handleTransform.position;
        Vector3 p1 = handleTransform.position + rock.MidPoint;
        Vector3 p2 = handleTransform.position + rock.EndPoint;

        Handles.color = Color.white;
        Handles.DrawLine(p0, p1);
        Handles.color = Color.cyan;
        Handles.DrawLine(p1, p2);

        Handles.color = Color.red;
        int steps = 100;
        Vector3 lastPoint = p0;
        for (int i = 1; i <= steps; i++) {
            Vector3 temp = Bezier.GetPoint(p0, p1, p2, (float)i / (float)steps);
            Handles.DrawLine(lastPoint, temp);
            lastPoint = temp;
        }
    }
}