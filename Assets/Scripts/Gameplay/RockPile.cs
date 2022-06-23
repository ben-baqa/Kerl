using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the available rocks
/// </summary>
public class RockPile : MonoBehaviour
{
    public GameObject rockPrefab;
    public Vector3 startingPosition;
    public Vector2 placementOffset;
    public Material blueRockMaterial;
    public Material redRockMaterial;

    List<GameObject> blueRocks = new List<GameObject>();
    List<GameObject> redRocks = new List<GameObject>();

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(.4f, 1f, .8f, 1f);

        Vector3 xOff = Vector3.right * placementOffset.x;
        Vector3 zOff = Vector3.forward * placementOffset.y;
        Vector3 position = transform.position + startingPosition;
        Vector3 size = Vector3.one;
        for (int i = 0; i < 5; i++)
        {
            Gizmos.DrawCube(position, size);
            Gizmos.DrawCube(position + xOff, size);
            position += zOff;
        }
    }

    public void PlaceRocks(int count)
    {
        // TODO: include placement of special rocks through MenuSelections
        // + movement of rocks when one is taken
        // (probably just remove all and redo placement

        Vector3 xOff = Vector3.right * placementOffset.x;
        Vector3 zOff = Vector3.forward * placementOffset.y;
        Vector3 position = transform.position + startingPosition;
        for(int i = 0; i < count; i++)
        {
            redRocks.Add(PlaceRock(position, redRockMaterial));
            blueRocks.Add(PlaceRock(position + xOff, blueRockMaterial));
            position += zOff;
        }
    }

    GameObject PlaceRock(Vector3 pos, Material mat)
    {
        GameObject newRock = Instantiate(rockPrefab, transform);
        newRock.GetComponent<MeshRenderer>().materials[1] = mat;
        newRock.transform.position = pos;
        return newRock;
    }

    public void RemoveRock(bool blueTurn)
    {
        GameObject toRemove = redRocks[redRocks.Count - 1];
        if (blueTurn)
        {
            toRemove = blueRocks[blueRocks.Count - 1];
            blueRocks.Remove(toRemove);
        }
        else
        {
            redRocks.Remove(toRemove);
        }
        Destroy(toRemove);
    }
}
