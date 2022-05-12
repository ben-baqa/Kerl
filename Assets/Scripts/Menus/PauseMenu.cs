using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Runs the pause menu logic
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject check;

    private Canvas menu;
    private Skipper skipper;

    // Start is called before the first frame update
    void Start()
    {
        menu = GetComponentInChildren<Canvas>();
        menu.enabled = false;
        skipper = FindObjectOfType<Skipper>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
        {
            menu.enabled = !menu.enabled;
            if (menu.enabled)
                Time.timeScale = 0;
            else
                Time.timeScale = 1;
        }
    }

    public void AdjustVolume(float v)
    {
        AudioListener.volume = v;
    }

    public void AdjustCurveSpeed(float v)
    {
        skipper.period = 1 / v;
    }

    public void OnClickAimAssist()
    {
        skipper.weightedCurve = !skipper.weightedCurve;
        check.SetActive(skipper.weightedCurve);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
