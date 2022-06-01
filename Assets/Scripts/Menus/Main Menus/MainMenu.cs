using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public enum MenuState
    {
        TITLE,
        //PLAY_OR_CUSTOMIZE,
        //CUSTOMIZATION,
        JOIN_MENU,
        //GAMEMODE_SELECT,
        CHARACTER_SELECT,
        ROCK_SELECT,
        //MAP_SELECT
    }

    MenuState state = MenuState.TITLE;

    public GameObject titleScreen;
    public GameObject playOrCustomize;
    public GameObject customization;
    public JoinMenu joinMenu;
    public GameObject gamemodeSelect;
    public GameObject characterSelect;
    public GameObject rockSelect;
    public GameObject mapSelect;

    // Start is called before the first frame update
    void Start()
    {
        ApplyState();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Continue()
    {
        state += 1;
        if (state > MenuState.ROCK_SELECT)
        {
            // load into game
            Debug.LogWarning("You need to implement this LMAO");
        }
        ApplyState();
    }

    void ApplyState()
    {
        titleScreen.SetActive(state == MenuState.TITLE);
        //playOrCustomize.SetActive(state == MenuState.PLAY_OR_CUSTOMIZE);
        //customization.SetActive(state == MenuState.CUSTOMIZATION);
        joinMenu.gameObject.SetActive(state == MenuState.JOIN_MENU);
        //gamemodeSelect.SetActive(state == MenuState.GAMEMODE_SELECT);
        characterSelect.SetActive(state == MenuState.CHARACTER_SELECT);
        rockSelect.SetActive(state == MenuState.ROCK_SELECT);
        //mapSelect.SetActive(state == MenuState.MAP_SELECT);
    }

    public void SetState(MenuState newState)
    {
        state = newState;
        ApplyState();
    }
}
