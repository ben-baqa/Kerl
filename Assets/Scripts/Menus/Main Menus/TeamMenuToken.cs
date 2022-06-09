using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TeamMenuToken : MonoBehaviour
{
    public Vector2 targetPosition;
    public int playerIndex, selection;

    PlacementGrid gridHost;
    Image image;
    Vector2 position;

    float lerp;
    
    public void Init(int index, Vector2 position, Sprite sprite, float lerpVal)
    {
        lerp = lerpVal;
        playerIndex = index;
        image = GetComponent<Image>();
        image.sprite = MenuSelections.GetInputSprite(index);

        GetComponentInChildren<TextMeshProUGUI>().text = InputProxy.GetInputInfo(index).name;

        this.position = position;
        transform.localPosition = position;
    }

    void FixedUpdate()
    {
        position = Vector2.Lerp(position, targetPosition, lerp);
        transform.localPosition = position;
    }

    public bool ChangeSelection(PlacementGrid grid, int newSelection)
    {
        if (selection == newSelection)
            return false;

        selection = newSelection;

        gridHost?.Remove(this);
        gridHost = grid;
        grid.Add(this);

        return true;
    }
}