using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushingCamera : MonoBehaviour
{
    public float followLerp = .05f;
    public float xOffset = 5.5f;
    public float zOffset = 1.5f;

    Transform cameraTransform;

    bool followRock;


    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPuch(Transform start)
    {

    }
}
