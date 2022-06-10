using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Reloads the initial menu from the results view after a game
/// </summary>
public class LoadSceneOnInput : MonoBehaviour
{
    public string sceneName = "Main Menu";

    private bool loaded;

    // Update is called once per frame
    void Update()
    {
        if (!loaded && InputProxy.any)
        {
            SceneManager.LoadScene(sceneName);
            loaded = true;
        }
    }
}
