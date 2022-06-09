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
        TEAM_MENU,
        CHARACTER_SELECT,
        ROCK_SELECT,
        //MAP_SELECT
    }

    MenuState state = MenuState.TITLE;

    [Header("Submenus")]
    public GameObject titleScreen;
    public GameObject playOrCustomize;
    public GameObject customization;
    public JoinMenu joinMenu;
    public GameObject gamemodeSelect;
    public GameObject teamMenu;
    public GameObject characterSelect;
    public GameObject rockSelect;
    public GameObject mapSelect;

    [Header("Input Sprites")]
    public Sprite joinSprite;
    public Sprite keySprite;
    public Sprite gamepadSprite;
    public Sprite bciSprite;
    public Sprite networkSprite;

    public static Dictionary<InputType, Sprite> inputSprites = new Dictionary<InputType, Sprite>();

    
    void Start()
    {
        ApplyState();

        inputSprites[InputType.invalid] = joinSprite;
        inputSprites[InputType.key] = keySprite;
        inputSprites[InputType.gamepad] = gamepadSprite;
        inputSprites[InputType.bci] = bciSprite;
        inputSprites[InputType.network] = networkSprite;
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

    public void Back()
    {
        state -= 1;
        ApplyState();
    }

    void ApplyState()
    {
        titleScreen.SetActive(state == MenuState.TITLE);
        //playOrCustomize.SetActive(state == MenuState.PLAY_OR_CUSTOMIZE);
        //customization.SetActive(state == MenuState.CUSTOMIZATION);
        joinMenu.gameObject.SetActive(state == MenuState.JOIN_MENU);
        //gamemodeSelect.SetActive(state == MenuState.GAMEMODE_SELECT);
        teamMenu.gameObject.SetActive(state == MenuState.TEAM_MENU);
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
