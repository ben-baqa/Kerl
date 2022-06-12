using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamMenu : MonoBehaviour
{
    public enum TeamCount { two = 2, four = 4 }
    public TeamCount teamCount = TeamCount.two;
    [Header("References")]
    public Transform twoTeamObject;
    public Transform fourTeamObject;
    public Transform tokenParent;
    public GameObject tokenPrefab;
    public TextMeshProUGUI timerText;
    [Header("Settings")]
    public Color activeColor;
    public Color inactiveColor;
    public float interval = 2;
    public float itemLerp;
    public float gridOffset;

    float timer = 0;
    float readyTimer = 0;
    int selectionIndex = 0;

    PlacementGrid centreGrid;
    List<PlacementGrid> selectionGrids;
    List<TeamMenuToken> tokens;

    GameObject activeGridObject;

    void Start()
    {
        twoTeamObject.gameObject.SetActive(false);
        fourTeamObject.gameObject.SetActive(false);
        timerText.text = "";
    }

    private void OnEnable()
    {
        ConstructMenu();
    }

    private void OnDisable()
    {
        // destroy all previous tokens
        foreach (Transform child in tokenParent)
            Destroy(child.gameObject);
    }

    void Update()
    {
        if (!activeGridObject.activeSelf)
            activeGridObject.SetActive(true);

        if (IsValidConfiguration())
        {
            readyTimer -= Time.deltaTime;
            timerText.text = "" + ((int)readyTimer + 1);

            if (readyTimer <= 0)
            {
                FindObjectOfType<MainMenu>().Continue();

                List<List<int>> teams = new List<List<int>>();
                foreach (PlacementGrid grid in selectionGrids)
                    teams.Add(grid.GetPlayers());
                MenuSelections.teams = teams;
            }
        }
        else if (readyTimer < interval * (int) teamCount)
        {
            readyTimer = interval * (int) teamCount;
            timerText.text = "";
        }

        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer -= interval;

            UpdateSelection();
        }

        for (int i = 0; i < InputProxy.playerCount; i++)
            if (InputProxy.GetToggledInput(i))
                Select(i);
    }

    void ConstructMenu()
    {
        // select which grid set will be used
        Transform active = twoTeamObject;
        if (teamCount == TeamCount.four)
            active = fourTeamObject;
        active.gameObject.SetActive(true);
        // grey out all grids
        foreach (Transform child in active)
            child.GetComponent<Image>().color = inactiveColor;
        activeGridObject = active.gameObject;

        // initiate starting grid
        bool verticalGrids = teamCount == TeamCount.two;
        centreGrid = new PlacementGrid(transform.position, gridOffset, verticalGrids);

        // initiate team grids
        selectionGrids = new List<PlacementGrid>();
        for (int i = 0; i < (int)teamCount; i++)
            selectionGrids.Add(new PlacementGrid(
                active.GetChild(i).localPosition, gridOffset, verticalGrids));

        // create and initialize tokens
        tokens = new List<TeamMenuToken>();
        for (int i = 0; i < InputProxy.playerCount; i++)
        {
            TeamMenuToken item = Instantiate(tokenPrefab, tokenParent).GetComponent<TeamMenuToken>();
            item.Init(i, transform.position, MenuSelections.GetInputSprite(i), itemLerp);
            tokens.Add(item);
            item.ChangeSelection(centreGrid, -1);
        }

        if (teamCount == TeamCount.two)
            SetOption(twoTeamObject, 0);
        else if (teamCount == TeamCount.four)
            SetOption(fourTeamObject, 0);
    }

    void UpdateSelection()
    {
        int previousIndex = selectionIndex;
        selectionIndex++;
        if (selectionIndex == (int)teamCount)
            selectionIndex -= (int)teamCount;

        if (teamCount == TeamCount.two)
            SetOption(twoTeamObject, previousIndex);
        else if (teamCount == TeamCount.four)
            SetOption(fourTeamObject, previousIndex);
    }

    void SetOption(Transform t, int previousIndex)
    {
        t.GetChild(previousIndex).GetComponent<Image>().color = inactiveColor;
        t.GetChild(selectionIndex).GetComponent<Image>().color = activeColor;
    }

    void Select(int playerIndex)
    {
        if (tokens[playerIndex].ChangeSelection(selectionGrids[selectionIndex], selectionIndex))
        {
            // reset timer if selection has changed
            readyTimer = interval * (int)teamCount;
            timerText.text = "";
        }
        
    }

    bool IsValidConfiguration()
    {
        foreach (var grid in selectionGrids)
            if (!grid.isValid)
                return false;

        return centreGrid.isEmpty;
    }
}
