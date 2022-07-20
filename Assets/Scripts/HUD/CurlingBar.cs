using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the arrival predition bar during brushing
/// </summary>
public class CurlingBar : MonoBehaviour
{
    public GameObject fill;
    public Gradient gradient;

    UIPredictionLine predictionLine;
    Slider slider;
    Image image;

    float _progress;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        image = fill.GetComponent<Image>();
        predictionLine = GetComponentInChildren<UIPredictionLine>(true);
    }

    public float Progress
    {
        set
        {
            slider.value = value;
            image.color = gradient.Evaluate(value);
            _progress = value;
        }
        get => _progress;
    }

    public void UpdatePredictionLine(List<Vector2> points)
    {
        predictionLine.DrawLines(points.Select(p => new Vector2(
            p.x * GetComponent<RectTransform>().rect.width,
            p.y * GetComponent<RectTransform>().rect.height)).ToList());
    }
}
