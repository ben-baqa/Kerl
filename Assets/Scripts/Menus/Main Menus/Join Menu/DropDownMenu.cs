using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropDownMenu : MonoBehaviour, IPointerEnterHandler
{
    bool open = false;

    public GameObject elementPrefab;
    public Transform elementParent;
    public Vector2 spacing = Vector2.right * 100;
    public Color highlightColour;

    List<JoinMenuDeleteButton> elements = new List<JoinMenuDeleteButton>();
    Image image;

    void OnEnable()
    {
        if (!image)
            image = GetComponent<Image>();

        InputProxy.InputAdded += OnInputAdded;
    }
    private void OnDisable()
    {
        InputProxy.InputAdded -= OnInputAdded;
    }

    public void OnPointerEnter(PointerEventData data)
    {
        Open();
    }

    void ReconstructMenu()
    {
        foreach (JoinMenuDeleteButton b in elements)
            Destroy(b.gameObject);
        elements.Clear();

        for (int i = 0; i < InputProxy.playerCount; i++)
        {
            JoinMenuDeleteButton b = Instantiate(elementPrefab, transform).GetComponent<JoinMenuDeleteButton>();
            b.transform.localPosition = spacing * (i + 1);
            b.Init(i, ReconstructMenu);
            elements.Add(b);
        }
        SetOpen();
    }

    public void Open()
    {
        open = true;
        SetOpen();
    }

    public void Close()
    {
        open = false;
        SetOpen();
    }

    void SetOpen()
    {
        foreach (JoinMenuDeleteButton b in elements)
            b.gameObject.SetActive(open);

        image.color = open ? highlightColour : Color.white;
    }

    void OnInputAdded(InputInfo info) => ReconstructMenu();
}
