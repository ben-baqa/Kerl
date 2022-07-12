using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TeamIntro : MonoBehaviour
{
    public Action Complete;

    public SimpleCameraTransition[] transitions;
    public Placement[] playerPlacements;

    Transform cameraTransform;

    int transitionIndex = 0;
    bool active;

    private void OnDrawGizmos()
    {
        if (playerPlacements == null)
            return;
        foreach (Placement p in playerPlacements)
        {
            Gizmos.matrix = Matrix4x4.identity;
            Gizmos.matrix *= Matrix4x4.Translate(transform.position + p.position);
            Gizmos.matrix *= Matrix4x4.Rotate(Quaternion.Euler(0, p.rotation, 0));
            Gizmos.DrawCube(Vector3.up * 0.5f, new Vector3(0.2f, 1, 0.5f));
        }
    }

    void Update()
    {
        if (active)
        {
            // TODO: add actual intro sequence
            if (transitions[transitionIndex].Process())
                StartNextTransition(++transitionIndex);
        }
    }

    public void StartSequence()
    {
        active = true;

        cameraTransform = Camera.main.transform;
        transitionIndex = 0;
        StartNextTransition(transitionIndex);
    }

    private void StartNextTransition(int index)
    {
        if(index >= transitions.Length)
        {
            active = false;
            Complete();
            return;
        }
        transitions[index].Start(cameraTransform);
    }

    public Placement[] GetPlayerPlacments()
    {
        Placement[] offsetPlacements = new Placement[playerPlacements.Length];
        for(int i = 0; i < playerPlacements.Length; i++)
        {
            Placement p = new Placement();
            p.position = transform.position + playerPlacements[i].position;
            p.rotation = playerPlacements[i].rotation;
            offsetPlacements[i] = p;
        }
        return offsetPlacements;
    }
}
