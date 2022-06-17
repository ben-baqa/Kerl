using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestGrid : MonoBehaviour
{
    public void OnNodeSelected(int color) {
        Debug.Log("player " + color + " just selected a node");
    }
}
