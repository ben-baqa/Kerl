using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSelector : MonoBehaviour
{
    RockPile rockPile;

    public GameObject redRockPrefab;
    public GameObject blueRockPrefab;

    Rock selectedRock;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartSelecting()
    {

    }

    void OnSelect()
    {
        RoundManager.instance.OnRockSelect(selectedRock);
    }
}
