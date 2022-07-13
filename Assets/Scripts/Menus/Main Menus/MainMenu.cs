using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public enum MenuState
    {
        Title,
        //PlayOrCustomize,
        //Customization,
        JoinMenu,
        //GamemodeSelection,
        TeamMenu,
        CharacterSelect,
        //RockSelect,
        //MapSelect
    }

    MenuState state = MenuState.Title;

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

    
    void Start()
    {
        ApplyState();
    }


    public void Continue()
    {
        state += 1;
        if (state > MenuState.CharacterSelect)
        {
            // load into game
            //Debug.LogWarning("You need to implement this LMAO");
            SceneManager.LoadScene("Worm Map");
        }
        ApplyState();
    }

    public void Back()
    {
        state -= 1;
        ApplyState();
    }

    public void Quit()
    {
        Application.Quit();
    }

    void ApplyState()
    {
        titleScreen.SetActive(state == MenuState.Title);
        //playOrCustomize.SetActive(state == MenuState.PlayOrCustomize);
        //customization.SetActive(state == MenuState.Customization);
        joinMenu.gameObject.SetActive(state == MenuState.JoinMenu);
        //gamemodeSelect.SetActive(state == MenuState.GamemodeSelection);
        teamMenu.gameObject.SetActive(state == MenuState.TeamMenu);
        characterSelect.SetActive(state == MenuState.CharacterSelect);
        //rockSelect.SetActive(state == MenuState.RockSelect);
        //mapSelect.SetActive(state == MenuState.MapSelect);
    }

    public void SetState(MenuState newState)
    {
        state = newState;
        ApplyState();
    }
    public void SetState(int newState)
    {
        state = (MenuState)newState;
        ApplyState();
    }
}
