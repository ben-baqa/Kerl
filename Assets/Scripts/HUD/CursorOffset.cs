using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CursorOffset : MonoBehaviour
{
    public static bool active
    {
        get
        {
            return _active;
        }
        set
        {
            if (value)
            {
                if (--disableCount == 0)
                    _active = true;
            }
            else
            {
                disableCount++;
                _active = false;
            }
        }
    }
    static bool _active = true;
    static int disableCount = 0;

    public float scale;

    Material material;
    Vector2 position = Vector2.zero;

    void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    void Update()
    {
        if (!active)
            return;

        position += Mouse.current.delta.ReadValue() * scale;

        material.SetVector("_offset", position);
    }
}
