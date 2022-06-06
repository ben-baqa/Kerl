using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamMenu : MonoBehaviour
{
    public enum TeamCount { two=2, four=4 }
    public TeamCount teamCount = TeamCount.two;
    public Transform twoTeamObject, fourTeamObject;

    public float interval = 2;

    float timer = 0;
    int selectionIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        twoTeamObject.gameObject.SetActive(false);
        fourTeamObject.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer >= interval)
        {
            timer -= interval;

            UpdateSelection();
        }

        for(int i = 0; i < InputProxy.playerCount; i++)
            if (InputProxy.GetToggledInput(i))
                Select(i);
    }

    void ConstructMenu()
    {
        if (teamCount == TeamCount.two)
            twoTeamObject.gameObject.SetActive(true);
        else if (teamCount == TeamCount.four)
            fourTeamObject.gameObject.SetActive(true);
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
        t.GetChild(previousIndex).gameObject.SetActive(false);
        t.GetChild(selectionIndex).gameObject.SetActive(true);
    }

    void Select(int playerIndex)
    {

    }

    [System.Serializable]
    public class GridPlacer
    {

    }
}
