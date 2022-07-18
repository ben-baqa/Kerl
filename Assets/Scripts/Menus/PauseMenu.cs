using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Runs the pause menu logic
/// </summary>
public class PauseMenu : MonoBehaviour
{
    //public GameObject check;
    public string menuScene = "Main Menu";
    public float aimSpeedMultiplier = 1;
    public float rockSpeedMultiplier = 2;

    Canvas canvas;
    Thrower thrower;
    Sweeper sweeper;

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        canvas.enabled = false;

        thrower = FindObjectOfType<Thrower>();
        sweeper = FindObjectOfType<Sweeper>();

        Slider[] sliders = GetComponentsInChildren<Slider>();
        sliders[0].value = AudioListener.volume;
        sliders[1].value = thrower.aimMovingSpeed * aimSpeedMultiplier;
        sliders[2].value = rockSpeedMultiplier / sweeper.rockTravelTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
            TogglePause();
    }

    public void TogglePause()
    {
        canvas.enabled = !canvas.enabled;
        if (canvas.enabled)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void AdjustVolume(float v)
    {
        AudioListener.volume = v;
    }

    public void AdjustCurveSpeed(float v)
    {
        thrower.aimMovingSpeed = v * aimSpeedMultiplier;
    }

    public void AdjustRockSpeed(float v)
    {
        sweeper.rockTravelTime = rockSpeedMultiplier / v;
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene(menuScene);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
