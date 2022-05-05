using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageHandler : MonoBehaviour
{
    public static bool isHost;
    public bool debug, menu;

    private SelectCharacter joinMenu;
    private Skipper skipper;
    private Sweeper sweeper;

    // Start is called before the first frame update
    void Start()
    {
        if (menu)
        {
            joinMenu = FindObjectOfType<SelectCharacter>();
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


    // Update is called once per frame
    void Update()
    {
        if (debug)
        {
            if (Input.GetKeyDown(KeyCode.H))
                GetComponent<BCIDataListener>().SetHost(true);
            if (Input.GetKeyDown(KeyCode.T))
                DataSender.Instance.SendToJS("TEST");
        }
    }
}
