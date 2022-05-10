using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class used to faciliate the sending and receipt of
/// multiplayer Network messages
/// </summary>
public class NetworkMessageHandler : MonoBehaviour
{
    /// <summary>
    /// Designates this instance of the application as the authoritative host
    /// set as true by default so the game will fucntion normally during development
    /// </summary>
    public static bool isHost = true;
    /// <summary>
    /// indicates game state and thus how incoming messages whould be handled
    /// </summary>
    private bool menu;

    // references needed to drive functionality
    private JoinMenu joinMenu;
    private Skipper skipper;
    private Sweeper sweeper;

    // Start is called before the first frame update
    void Start()
    {
        if (menu)
        {
            joinMenu = FindObjectOfType<JoinMenu>();
        }
        else
        {
            skipper = FindObjectOfType<Skipper>();
            sweeper = FindObjectOfType<Sweeper>();
        }
    }

    // Act on message from session host
    public void HandleMessage(string s)
    {
        if (menu) MenuMessage(s);
        else GameMessage(s);
    }

    /* Handle messages in Menu
     * formats:
     * join [#]
     * ready [#]
    */
    private void MenuMessage(string s)
    {
        string[] ar = s.Split();
        switch (ar[0])
        {
            case "join":
                joinMenu.SetState(int.Parse(ar[1]));
                break;

            case "ready":
                joinMenu.SetState(int.Parse(ar[1]), true);
                break;

            default:
                print("Invalid message format");
                break;
        }
    }

    /* Handle messages in Game
     * formats:
     * throw [angle]
     * sweep
     * menu
    */
    private void GameMessage(string s)
    {
        string[] ar = s.Split();
        switch (ar[0])
        {
            case "throw":
                skipper.Throw(float.Parse(ar[1]));
                break;

            case "sweep":
                sweeper.Sweep();
                break;

            case "menu":
                break;

            default:
                print("Invalid message format");
                break;
        }
    }
}
