using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorOffset : MonoBehaviour
{
    public float scale;

    Material material;
    Vector2 position = Vector2.zero;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        position += Mouse.current.delta.ReadValue() * scale;

        material.SetVector("_offset", position);
    }
}
