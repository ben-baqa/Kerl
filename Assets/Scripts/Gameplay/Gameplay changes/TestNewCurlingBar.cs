using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class TestNewCurlingBar : MonoBehaviour
{
    public TestThrower Thrower;

    private Vector3 anchor;
    private Vector3 upLeft;
    private Vector3 downRight;

    [Range(0.0f, 1.0f)]
    public float Progress;

    public GameObject Fill;
    public UIPredictLine PredictLine;
    public Gradient Gradient;

    private Slider slider;
    private Image image;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        image = Fill.GetComponent<Image>();

        anchor = Thrower.ForwardDirection - ((Quaternion.Euler(0, 90, 0) * Thrower.ForwardDirection).normalized + Thrower.ForwardDirection.normalized * 3) * (Thrower.TargetDiameter / 2);
        upLeft = Thrower.ForwardDirection.normalized * Thrower.TargetDiameter * 2;
        downRight = (Quaternion.Euler(0, 90, 0) * Thrower.ForwardDirection).normalized * Thrower.TargetDiameter;
    }

    void Update()
    {
        Progress = Thrower.progress;

        slider.value = Progress;
        image.color = Gradient.Evaluate(Progress);

        if (Thrower.throwState == TestThrower.ThrowState.Brushing)
        {
            Vector3[] originalPoints = Thrower.GetPredictionPoints();
            List<Vector2> points = new List<Vector2>();
            for (int i = 0; i < originalPoints.Length; i++)
            {
                    points.Add(ConvertToUI(originalPoints[i]));
            }
            PredictLine.DrawLines(points);
        }
    }

    private Vector2 ConvertToUI(Vector3 original) {
        return new Vector2(GetComponent<RectTransform>().rect.width * Vector3.Project(original - anchor, downRight).magnitude / downRight.magnitude, GetComponent<RectTransform>().rect.height * Vector3.Project(original - anchor, upLeft).magnitude / upLeft.magnitude);
    }
}

[CustomEditor(typeof(TestNewCurlingBar))]
public class TestNewCurlingBarEditor : Editor
{
    private void OnSceneGUI()
    {
        TestNewCurlingBar bar = target as TestNewCurlingBar;
        TestThrower thrower = bar.Thrower;

        Vector3 p0 = thrower.transform.position + thrower.ForwardDirection - ((Quaternion.Euler(0, 90, 0) * thrower.ForwardDirection).normalized + thrower.ForwardDirection.normalized * 3) * (thrower.TargetDiameter / 2);
        Vector3 p1 = thrower.ForwardDirection.normalized * thrower.TargetDiameter * 2;
        Vector3 p2 = (Quaternion.Euler(0, 90, 0) * thrower.ForwardDirection).normalized * thrower.TargetDiameter;

        Handles.color = Color.white;
        Handles.DrawLine(p0, p0 + p1);
        Handles.DrawLine(p0, p0 + p2);
    }
}