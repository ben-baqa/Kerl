using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public int playerCount;
    public bool on;

    private enum Turn { p1, p2, p3, p4, com}
    private Turn turn = Turn.p1;

    private InputProxy proxy;

    private bool blueTurn = true, skipper = true;

    // Start is called before the first frame update
    void Start()
    {
        proxy = GetComponent<InputProxy>();
        playerCount = Mathf.Clamp(playerCount, 1, 4);
    }

    // Update is called once per frame
    void Update()
    {
        on = GetInput();
    }

    public void OnTurn()
    {
        blueTurn = !blueTurn;
        if (blueTurn)
            skipper = !skipper;

        if(playerCount == 1)
        {
            turn = blueTurn ? Turn.p1 : Turn.com;
        }
        else if(playerCount == 2)
        {
            if (blueTurn)
                turn = skipper ? Turn.p1 : Turn.p2;
            else
                turn = Turn.com;
        }
        else if(playerCount == 3)
        {
            if (blueTurn)
                turn = skipper ? Turn.p1 : Turn.p2;
            else
                turn = Turn.p3;
        }
        else if(playerCount == 4)
        {
            if (blueTurn)
                turn = skipper ? Turn.p1 : Turn.p2;
            else
                turn = skipper ? Turn.p3 : Turn.p4;
        }
    }

    public void OnThrow()
    {
        if (playerCount > 1)
        {
            if (blueTurn)
                turn = skipper ? Turn.p2 : Turn.p1;
        }
        if (playerCount == 4 && !blueTurn)
        {
            turn = skipper ? Turn.p4 : Turn.p3;
        }
    }

    public bool GetInput()
    {
        switch (turn)
        {
            case Turn.p1:
                return proxy.p1;
            case Turn.p2:
                return proxy.p2;
            case Turn.p3:
                return proxy.p3;
            case Turn.p4:
                return proxy.p4;
            case Turn.com:
                return true;
        }
        return false;
    }
}
