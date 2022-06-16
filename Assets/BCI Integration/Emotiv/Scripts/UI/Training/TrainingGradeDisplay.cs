using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TrainingGradeDisplay : MonoBehaviour
{
    public GradeThreshold[] thresholds;
    public Gradient colorGradient;

    TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    public void SetGrade(float n)
    {
        GradeThreshold grade = thresholds[0];
        foreach(GradeThreshold g in thresholds)
        {
            if (g.threshold > n)
                break;
            grade = g;
        }

        if (!text)
            text = GetComponent<TextMeshProUGUI>();

        text.text = grade.grade;
        text.color = colorGradient.Evaluate(n);
    }

    [System.Serializable]
    public class GradeThreshold
    {
        public float threshold = 0.5f;
        public string grade = "F";
    }
}
