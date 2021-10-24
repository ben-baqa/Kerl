using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurveLine : MonoBehaviour
{
    public int segments = 10;
    public float distance = 35;

    private LineRenderer line;

    private Vector3 start, end;

    //private Vector3 c;
    //private float r;

    // Start is called before the first frame update
    void Start()
    {
        line = GetComponent<LineRenderer>();
        start = Vector3.zero;
        end = Vector3.forward * distance;
    }

    public void Generate(float angle)
    {
        angle *= Mathf.Deg2Rad;
        float d = distance / 2;

        Vector3 centre = start +
            Vector3.right * d / Mathf.Tan(angle) +
            Vector3.forward * d;

        float radius = (centre - start).magnitude;

        Vector3[] positions = new Vector3[segments + 1];
        positions[0] = start;
        positions[segments] = end;

        bool flip = false;
        if (angle < 0)
        {
            flip = true;
            angle *= -1;
        }

        float diff = angle * 2 / segments;
        float a = angle - diff;
        int index = 1;
        while(a > -angle)
        {
            positions[index] = centre + (flip? 1: -1) *
                radius * Vector3.right * Mathf.Cos(a) -
                radius * Vector3.forward * Mathf.Sin(a);

            a -= diff;
            index++;
        }
        //string s = "";
        //foreach (Vector3 v in positions)
        //    s += v + ",  ";
        //print(s);

        line.SetPositions(positions);

        //Debug.DrawLine(centre, start, Color.black, Time.deltaTime);
        //Debug.DrawLine(centre, end, Color.black, Time.deltaTime);
        //c = centre;
        //r = radius;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireSphere(c, r);
    //}
}
