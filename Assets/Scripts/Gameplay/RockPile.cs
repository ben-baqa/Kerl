using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keeps track of the available rocks
/// </summary>
public class RockPile : MonoBehaviour
{
    public List<GameObject> blueRocks, redRocks;

    private bool blue = true;
    
    public void OnTurnStart()
    {
        if (blue)
        {
            GameObject r = blueRocks[blueRocks.Count - 1];
            blueRocks.Remove(r);
            Destroy(r);
        }
        else
        {
            GameObject r = redRocks[redRocks.Count - 1];
            redRocks.Remove(r);
            Destroy(r);
        }

        blue = !blue;
    }
}
