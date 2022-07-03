using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrushingCamera : MonoBehaviour
{
    public float followLerp = .05f;
    public Vector3 positionOffset;
    public Vector3 lookOffset;

    Transform rock;
    Vector3 pos = Vector3.zero;

    [HideInInspector]
    public bool followRock = false;

    void Start()
    {
        followRock = false;
    }

    void Update()
    {
        if (!followRock)
            return;

        pos.z = Mathf.Lerp(pos.z, rock.position.z + positionOffset.z, followLerp);
        pos.x = rock.position.x + positionOffset.x;
        pos.y = rock.position.y + positionOffset.y;
        transform.position = Vector3.Lerp(transform.position, pos, followLerp);

        Quaternion targetRotation = Quaternion.LookRotation(rock.position + lookOffset - transform.position, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, .01f);
        //transform.LookAt(rock.position + lookOffset);
    }

    public void SetRock(Rock rockInstance)
    {
        rock = rockInstance.transform;
        pos = rock.position + positionOffset;
    }
}
