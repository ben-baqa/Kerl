using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the multuple camera views during gameplay
/// </summary>
public class CameraPositions : MonoBehaviour
{
    private static CameraPositions instance;

    public Transform cam, aiming, result, push;

    public float followLerp = .05f, xOffset = 5.5f, zOffset;

    private Transform rock;
    private Vector3 pos = Vector3.zero;
    private bool followRock;

    private void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    private void LateUpdate()
    {
        if (followRock)
        {
            pos.z = Mathf.Lerp(pos.z, rock.position.z + zOffset, followLerp);
            pos.x = rock.position.x + push.position.x;
            cam.position = pos;
        }
    }


    public static void OnTurnStart() { instance.ChangeView(instance.aiming); }
    public static void OnPush(Transform rock)
    {
        instance.ChangeView(instance.push, true, rock);
    }
    public static void OnResult() { instance.ChangeView(instance.result); }

    public void ChangeView(Transform t, bool b = false, Transform rock = null)
    {
        cam.position = t.position;
        cam.rotation = t.rotation;
        followRock = b;
        if (rock)
        {
            this.rock = rock;
            pos = t.position;
        }
    }

}
