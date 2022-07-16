using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SwitchSelectMenu : MonoBehaviour
{
    [Header("References")]
    public GameObject nodePrefab;
    public Transform nodeParent;
    public Image preview;
    public TextMeshProUGUI timerText;

    public LockableNode[] nodeInfo;

    [Header("Selection Settings")]
    public float unSelectedSizeRatio = 0.5f;
    public float selectionPeriod = 2;
    public float confirmationTime = 5;

    [Header("Token Settings")]
    public Transform tokenParent;
    public SelectionTokenSettings tokenSettings;

    SwitchSelectNode[] nodes;
    SelectionToken[] tokens;
    int[] selections;

    float selectionTimer;
    float confirmationTimer;
    float originaltextSize;
    int cursor;

    bool confirming;


    void Start()
    {
        nodes = new SwitchSelectNode[nodeInfo.Length];
        for(int i = 0; i < nodes.Length; i++)
        {
            GameObject nodeObject = Instantiate(nodePrefab, nodeParent, false);
            nodes[i] = nodeObject.GetComponent<SwitchSelectNode>();
            nodes[i].Init(nodeInfo[i], unSelectedSizeRatio);
        }
        nodes[0].Select();

        timerText.enabled = false;
        originaltextSize = timerText.fontSize;
    }

    void OnEnable()
    {
        int playerCount = InputProxy.playerCount;
        confirming = false;
        timerText.enabled = false;
        cursor = 0;
        preview.sprite = nodeInfo[0].Image;

        foreach (Transform child in tokenParent)
            Destroy(child.gameObject);

        selections = new int[playerCount];
        tokens = new SelectionToken[playerCount];
        for(int i = 0; i < playerCount; i++)
        {
            selections[i] = -1;
            tokens[i] = new SelectionToken(tokenParent, tokenSettings.size);
            tokens[i].Sprite = tokenSettings.offSprite;
            tokens[i].Colour = MenuSelections.GetColor(i);
            
            tokens[i].Enable(tokenSettings.neutralPosition +
                (i - ((playerCount - 1) / 2f)) * tokenSettings.neutralSpacing);
        }
    }

    void Update()
    {
        selectionTimer += Time.deltaTime;
        if(selectionTimer > selectionPeriod)
        {
            selectionTimer -= selectionPeriod;
            UpdateSelection();
        }

        for (int i = 0; i < InputProxy.playerCount; i++)
            if (InputProxy.GetToggledInput(i))
                Select(i);

        if (confirming)
        {
            confirmationTimer -= Time.deltaTime;
            timerText.fontSize = originaltextSize * (.8f + Mathf.Pow((confirmationTimer % 1), 3) / 5);
            timerText.text = "" + Mathf.CeilToInt(confirmationTimer);
            if (confirmationTimer < 0)
                FindObjectOfType<MainMenu>().Continue();
        }
    }

    void FixedUpdate()
    {
        foreach (SelectionToken token in tokens)
            token.Process();
    }

    void UpdateSelection()
    {
        do
        {
            nodes[cursor].Deselect();

            cursor++;
            cursor %= nodeInfo.Length;
            nodes[cursor].Select();
            if (!confirming)
                preview.sprite = nodeInfo[cursor].Image;
        }
        while (nodeInfo[cursor].locked);
    }

    void Select(int playerIndex)
    {
        if (selections[playerIndex] == cursor)
            return;

        int oldSelection = selections[playerIndex];
        selections[playerIndex] = cursor;

        ArrangeTokens(cursor);
        if(oldSelection >= 0)
            ArrangeTokens(oldSelection);

        CheckForMajority();
        timerText.enabled = confirming;
    }

    void ArrangeTokens(int cursor)
    {
        Vector2 position = nodes[cursor].Position + tokenSettings.selectedPosition;
        for(int i = 0; i < selections.Length; i++)
        {
            if (selections[i] == cursor)
            {
                tokens[i].target = position;
                tokens[i].Sprite = tokenSettings.onSprite;
                position += tokenSettings.selectedSpacing;
            }
        }
    }

    void CheckForMajority()
    {
        confirming = false;
        for (int i = 0; i < nodes.Length; i++)
        {
            int numberOfSelections = 0;
            foreach (int selection in selections)
                if (selection == i)
                    numberOfSelections++;

            if (numberOfSelections > InputProxy.playerCount / 2)
            {
                MenuSelections.map = nodeInfo[i].StringPayload;
                preview.sprite = nodeInfo[i].Image;
                confirmationTimer = confirmationTime;
                confirming = true;
                return;
            }
        }
        preview.sprite = nodeInfo[cursor].Image;
    }
}
