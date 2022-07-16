using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Runs the pause menu logic
/// </summary>
public class PauseMenu : MonoBehaviour
{
    public GameObject check;

    private Canvas menu;
    private Thrower thrower;

    // Start is called before the first frame update
    void Start()
    {
        menu = GetComponentInChildren<Canvas>();
        menu.enabled = false;
        thrower = FindObjectOfType<Thrower>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame ||
            Keyboard.current.pKey.wasPressedThisFrame)
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
        //thrower.period = 1 / v;
    }

    public void OnClickAimAssist()
    {
        //thrower.weightedCurve = !thrower.weightedCurve;
        //check.SetActive(thrower.weightedCurve);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
