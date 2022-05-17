using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DisplayBar : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float progress;
    public string displayText;

    public GameObject fill;
    public Gradient gradient;
    public TextMeshProUGUI text;

    private Slider slider;
    private Image image;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        image = fill.GetComponent<Image>();
    }

    void Update()
    {
        text.text = displayText;
        slider.value = progress;
        image.color = gradient.Evaluate(progress);
    }
}
