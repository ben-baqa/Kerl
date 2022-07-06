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
    private bool thrown = false;
    private bool inOrbit = true;


    private void FixedUpdate()
    {
        if (thrown)
        {
            traveled += velocity.magnitude * Time.deltaTime;
            GetComponent<Rigidbody>().MovePosition(transform.position + velocity * Time.deltaTime);
            velocity *= Mathf.Pow(Decay, Time.deltaTime);
            if (inOrbit)
            {
                Vector3 destination = Vector3.Lerp(MidPoint, EndPoint, traveled / MakeShiftBezierArclength(20));
                velocity = (destination - transform.localPosition).normalized * velocity.magnitude;
            }

            if (velocity.magnitude <= Threshold)
            {
                velocity = Vector3.zero;
            }
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

    public void Throw() {
        velocity = MidPoint.normalized * (Threshold - MakeShiftBezierArclength(100) * Mathf.Log(Decay));
        thrown = true;
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
