using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class RockSelector : MonoBehaviour
{
    RockPile rockPile;

    [Header("Option placement options")]
    public GameObject optionPrefab;
    public float optionOffset = 150;
    public Vector3 activePosition;
    public Vector3 hiddenPosition;
    public float hideLerp = 0.1f;

    [Header("Selection options")]
    public float delay = 5;
    public float fillMultiplier = 1.5f;
    public float startingAmount = 2;
    public float requiredAmount = 5;
    public Color disabledColor;

    [Header("references")]
    public GameObject redRockPrefab;
    public GameObject blueRockPrefab;

    public Sprite normalRockIcon;

    TurnManager turnManager;
    ProgressBar progressBar;

    List<List<NodeElement>> options;
    List<Option> activeOptions;

    int selection = 0;
    float selectionProgress;
    float timer;
    bool blue;
    bool active;

    void Start()
    {
        turnManager = FindObjectOfType<TurnManager>();
        progressBar = GetComponentInChildren<ProgressBar>();
        progressBar.SetProgress(0);

        transform.localPosition = hiddenPosition;

        SetOptions();
    }

    void SetOptions()
    {
        List<List<int>> teams = MenuSelections.teams;
        if (teams == null)
            teams = MenuSelections.debugTeams;

        if (MenuSelections.rockSelections == null)
        {
            // fake rock selections with empty selections (only basic rocks can be selected)
            // one for fake player 1, player 2, and one at end for ai
            options = new List<List<NodeElement>> {
                new List<NodeElement>(),
                new List<NodeElement>(),
                new List<NodeElement>()
            };
        }
        else
            options = new List<List<NodeElement>>(MenuSelections.rockSelections);
    }

    void SetOptionsForTeam(List<int> team, NodeElement rockNode)
    {
        if(team.Count == 0)
            options.Last().Add(rockNode);

        foreach(int index in team)
            options[index].Add(rockNode);
    }

    void Update()
    {
        transform.localPosition = Vector3.Lerp(transform.localPosition,
            active ? activePosition : hiddenPosition, hideLerp);

        if (!active)
            return;

        if(selectionProgress > 0)
        {
            if (turnManager.GetInput())
                selectionProgress += Time.deltaTime * fillMultiplier;
            else
                selectionProgress -= Time.deltaTime;

            if (selectionProgress >= requiredAmount)
                Select();
            if(selectionProgress < 0)
            {
                selectionProgress = 0;
                progressBar.Deactivate();
            }

            progressBar.SetProgress(selectionProgress / requiredAmount);
        }
        else
        {
            timer += Time.deltaTime;
            if(timer >= delay)
            {
                timer -= delay;
                activeOptions[selection].image.color = disabledColor;
                selection = (selection + 1) % activeOptions.Count;
                activeOptions[selection].image.color = Color.white;
            }

            if (turnManager.GetInput())
            {
                selectionProgress = startingAmount;
                progressBar.SetProgress(selectionProgress / requiredAmount);

                Vector3 pos = Vector3.zero;
                pos.x = progressBar.transform.localPosition.x;
                pos.y = activeOptions[selection].image.transform.localPosition.y;
                progressBar.transform.localPosition = pos;

                progressBar.Activate();

                timer = delay;
            }
        }
    }

    public void StartSelecting(bool blueTurn)
    {
        blue = blueTurn;
        int playerIndex = turnManager.CurrentPlayer;
        int optionIndex = (options.Count + playerIndex) % options.Count;

        activeOptions = new List<Option>();
        List<NodeElement> rocks = options[optionIndex];

        Vector3 position = Vector3.down * optionOffset * rocks.Count / 2;
        foreach(NodeElement rock in rocks)
        {
            InstantiateOption(position, rock.PrefabPayload, rock.Image);
            position += Vector3.up * optionOffset;
        }
        InstantiateOption(position, blue ? blueRockPrefab : redRockPrefab, normalRockIcon);

        active = true;
        selection = 0;
        selectionProgress = 0;
        progressBar.Deactivate();
        activeOptions[selection].image.color = Color.white;
    }

    void InstantiateOption(Vector3 position, GameObject prefab, Sprite sprite)
    {
        GameObject optionInstance = Instantiate(optionPrefab, transform);
        optionInstance.transform.localPosition = position;
        Image optionImage = optionInstance.GetComponent<Image>();
        optionImage.sprite = sprite;
        optionImage.color = disabledColor;
        activeOptions.Add(new Option(prefab, optionImage));
    }

    void Select()
    {
        active = false;
        Rock rock = Instantiate(activeOptions[selection].prefab).GetComponent<Rock>();
        RoundManager.instance.OnRockSelect(rock);
    }

    class Option
    {
        public Image image;
        public GameObject prefab;

        public Option(GameObject Prefab, Image Image)
        {
            prefab = Prefab;
            image = Image;
        }
    }
}
