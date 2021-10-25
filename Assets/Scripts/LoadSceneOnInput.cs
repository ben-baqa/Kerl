using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneOnInput : MonoBehaviour
{
    private InputProxy input;

    private bool loaded;

    private void Start()
    {
        input = FindObjectOfType<InputProxy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!loaded && (input.p1 || input.p2 || input.p3 || input.p4))
            SceneManager.LoadScene("Start");
    }
}
