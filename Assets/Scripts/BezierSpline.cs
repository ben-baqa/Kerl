using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Based on this Tutorial: https://catlikecoding.com/unity/tutorials/curves-and-splines/
// with added weights to smooth traversal
public class BezierSpline : MonoBehaviour
{
    public int CurveCount => (points.Length - 1) / 3;
    public int ControlPointCount => points.Length;

    const int ArcLengthSteps = 10;

    [SerializeField]
    Vector3[] points;
    [SerializeField]
    BezierControlPointMode[] modes;
    // anchor point weights based on the lengths of connected curves
    [SerializeField]
    float[] curveLengths;
    [SerializeField]
    float totalLength;
    [SerializeField]
    bool _loop;

    public bool Loop
    {
        get => _loop;
        set
        {
            _loop = value;
            if(value)
            {
                modes[modes.Length - 1] = modes[0];
                this[0] = points[0];
            }
        }
    }

    public Vector3 this[int index]
    {
        get => points[index];
        set 
        {
            Vector3 delta = value - points[index];
            if(index % 3 == 0)
            {
                if (_loop)
                {
                    if(index == 0)
                    {
                        points[1] += delta;
                        points[points.Length - 2] += delta;
                        points[points.Length - 1] = value;
                    }else if(index == points.Length - 1)
                    {
                        points[0] = value;
                        points[1] += delta;
                        points[index - 1] += delta;
                    }
                    else
                    {
                        points[index - 1] += delta;
                        points[index + 1] += delta;
                    }
                }
                else
                {
                    if (index > 0)
                        points[index - 1] += delta;
                    if (index + 1 < points.Length)
                        points[index + 1] += delta;
                }
            }

            points[index] = value;
            EnforceMode(index);
            RecalculateLength();
        }
    }

    public void Reset()
    {
        points = new Vector3[] {
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 2),
            new Vector3(0, 0, 3),
            new Vector3(0, 0, 4)
        };

        modes = new BezierControlPointMode[]
        {
            BezierControlPointMode.Free,
            BezierControlPointMode.Free
        };

        RecalculateLength();
    }

    public Vector3 GetPoint(float t)
    {
        int i;
        if(t >= 1)
        {
            t = 1;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetPoint(
            points[i], points[i + 1], points[i + 2], points[i + 3], t));
    }

    /// <summary>
    /// get point treating the whole curve as one coninuos line, rather than segments,
    /// returns a point t% of the way down the line by length, rather than by portion
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public Vector3 GetPointByLength(float t)
    {
        float length = t * totalLength;
        float newT = 0;
        float tInc = 1f / CurveCount;
        for (int i = 0; i < CurveCount; i++)
        {
            if(length <= curveLengths[i])
            {
                newT += tInc * length / curveLengths[i];
                return GetPoint(newT);
            }
            newT += tInc;
            length -= curveLengths[i];
        }
        return GetPoint(1);
    }

    public Vector3 GetVelocity (float t)
    {
        int i;
        if (t >= 1)
        {
            t = 1;
            i = points.Length - 4;
        }
        else
        {
            t = Mathf.Clamp01(t) * CurveCount;
            i = (int)t;
            t -= i;
            i *= 3;
        }

        return transform.TransformPoint(Bezier.GetFirstDerrivative(
            points[i], points[i + 1], points[i + 2], points[i + 3], t)) - transform.position;
    }

    public Vector3 GetDirection(float t)
    {
        return GetVelocity(t).normalized;
    }

    void RecalculateLength()
    {
        curveLengths = new float[CurveCount];
        totalLength = 0;
        for (int i = 0; i < CurveCount; i++)
        {
            float length = GetCurveLength(i * 3);
            curveLengths[i] = length;
            totalLength += length;
        }
    }

    // estimate the arc length of a segment containing or starting with the given point index
    public float GetCurveLength(int index)
    {
        int startIndex = (index / 3) * 3;
        float length = 0;

        Vector3 p0 = points[startIndex];
        Vector3 p1 = points[startIndex + 1];
        Vector3 p2 = points[startIndex + 2];
        Vector3 p3 = points[startIndex + 3];

        Vector3 startPoint = points[startIndex];
        Vector3 endPoint;
        for(int i = 1; i <= ArcLengthSteps; i++)
        {
            endPoint = Bezier.GetPoint(p0, p1, p2, p3, i / (float)ArcLengthSteps);
            length += Vector3.Distance(startPoint, endPoint);
            startPoint = endPoint;
        }

        return length;
    }

    public void AddCurve()
    {
        Vector3 point = points[points.Length - 1];
        Vector3 direction = points[points.Length - 1] - points[points.Length - 2];
        Array.Resize(ref points, points.Length + 3);
        point += direction;
        points[points.Length - 3] = point;
        point += direction;
        points[points.Length - 2] = point;
        point += direction;
        points[points.Length - 1] = point;

        Array.Resize(ref modes, modes.Length + 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];

        EnforceMode(points.Length - 4);
        RecalculateLength();

        if (_loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void RemoveEndCurve()
    {
        if (points.Length <= 4)
        {
            Reset();
            return;
        }
        Array.Resize(ref points, points.Length - 3);
        Array.Resize(ref modes, modes.Length - 1);
        modes[modes.Length - 1] = modes[modes.Length - 2];

        EnforceMode(points.Length - 4);
        RecalculateLength();

        if (_loop)
        {
            points[points.Length - 1] = points[0];
            modes[modes.Length - 1] = modes[0];
            EnforceMode(0);
        }
    }

    public void DeletePoint(int index)
    {
        if(index >= points.Length - 2)
        {
            RemoveEndCurve();
            return;
        }
        if (points.Length <= 4)
        {
            Reset();
            return;
        }
        int modeIndex = (index + 1) / 3;
        int pivotIndex = modeIndex * 3;

        Vector3[] newPoints = new Vector3[points.Length - 3];
        BezierControlPointMode[] newModes = new BezierControlPointMode[modes.Length - 1];

        int n = 0;
        for(int i = 0; i < newModes.Length; i++)
        {
            if (i == modeIndex)
            {
                n = 1;
            }
            newModes[i] = modes[i + n];

            int j = i * 3 - 1;
            if(j > 0)
                newPoints[j] = points[j + n * 3];
            j++;
            newPoints[j] = points[j + n * 3];
            j++;
            if (j < newPoints.Length)
                newPoints[j] = points[j + n * 3];
        }

        points = newPoints;
        modes = newModes;

        EnforceMode(modeIndex * 3);
        EnforceMode((modeIndex - 1) * 3);
        RecalculateLength();

        if (_loop && index < 2)
        {
            points[points.Length - 1] = points[0];
            EnforceMode(0);
        }
    }

    public BezierControlPointMode GetControlPointMode(int index) => modes[(index + 1) / 3];
    public void SetControlPointMode(int index, BezierControlPointMode mode)
    {
        int modeIndex = (index + 1) / 3;
        modes[modeIndex] = mode;

        if (_loop)
        {
            if (modeIndex == 0)
                modes[modes.Length - 1] = mode;
            else if (modeIndex == modes.Length - 1)
                modes[0] = mode;
        }

        EnforceMode(index);
        RecalculateLength();
    }

    void EnforceMode(int index)
    {
        int modeIndex = (index + 1) / 3;
        BezierControlPointMode mode = modes[modeIndex];
        if (mode == BezierControlPointMode.Free || !_loop && (modeIndex == 0 || modeIndex == modes.Length - 1))
            return;

        int middleIndex = modeIndex * 3;
        int fixedIndex, enforcedIndex;
        if(index < middleIndex)
        {
            fixedIndex = middleIndex - 1;
            if (fixedIndex < 0)
                fixedIndex = points.Length - 2;
            enforcedIndex = middleIndex + 1;
            if (enforcedIndex >= points.Length)
                enforcedIndex = 1;
        }
        else
        {
            fixedIndex = middleIndex + 1;
            if (fixedIndex >= points.Length)
                fixedIndex = 1;
            enforcedIndex = middleIndex - 1;
            if (enforcedIndex < 0)
                enforcedIndex = points.Length - 2;
        }

        Vector3 middle = points[middleIndex];
        Vector3 enforcedTangent = middle - points[fixedIndex];
        if (mode == BezierControlPointMode.Aligned)
            enforcedTangent = enforcedTangent.normalized * Vector3.Distance(middle, points[enforcedIndex]);
        points[enforcedIndex] = middle + enforcedTangent;
    }
}

public enum BezierControlPointMode
{
    Free, Aligned, Mirrored
}

public class Bezier
{
    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1 - t;
        return
            oneMinusT * oneMinusT * p0 +
            2 * oneMinusT * t * p1 +
            t * t * p2;
    }

    public static Vector3 GetFirstDerrivative(Vector3 p0, Vector3 p1, Vector3 p2, float t)
    {
        return
            2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
    }

    public static Vector3 GetPoint(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1 - t;
        return
            oneMinusT * oneMinusT * oneMinusT * p0 +
            3 * oneMinusT * oneMinusT * t * p1 +
            3 * oneMinusT * t * t * p2 +
            t * t * t * p3;
    }

    public static Vector3 GetFirstDerrivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1 - t;
        return
            3 * oneMinusT * oneMinusT * (p1 - p0) +
            6 * oneMinusT * t * (p2 - p1) +
            3 * t * t * (p3 - p2);
    }
}