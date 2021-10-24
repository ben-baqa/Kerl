using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurlingBar : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float progress;

    public GameObject fill;
    public Gradient gradient;

    private Slider slider;
    private Image image;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        image = fill.GetComponent<Image>();
    }

    void Update()
    {
        slider.value = progress;
        image.color = gradient.Evaluate(progress);
    }
}
