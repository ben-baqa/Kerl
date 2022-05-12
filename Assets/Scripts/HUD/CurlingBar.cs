using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Displays the arrival predition bar during brushing
/// </summary>
public class CurlingBar : MonoBehaviour
{
    [Range(0.0f, 1.0f)]
    public float progress;

    public GameObject fill;
    public Gradient gradient;

    private Slider slider;
    private Image image;
    private AudioSource twinkleNoise;

    void Start()
    {
        slider = gameObject.GetComponent<Slider>();
        image = fill.GetComponent<Image>();
        twinkleNoise = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (twinkleNoise)
            twinkleNoise.Play();
    }

    void Update()
    {
        slider.value = progress;
        image.color = gradient.Evaluate(progress);
        twinkleNoise.volume = 1 - Mathf.Pow(4 * progress - 3, 2);
    }
}
