using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public struct PlayerSelections
//{
//    public int team;
//    public GameObject character;
//    public List<NodeElement> rocks;
//}

/// <summary>
/// Stores the selections made on the main menu for use in game
/// </summary>
public class MenuSelections : MonoBehaviour
{
    public static MenuSelections instance;
    public static List<List<int>> teams;
    public static Dictionary<int, GameObject> characterSelections;
    public static List<List<NodeElement>> rockSelections;



    public static List<List<int>> debugTeams =
        new List<List<int>> { new List<int> { 0, 1 }, new List<int> { 2 } };

    public TypedInputSprites inputSprites;
    public PlayerColours playerColours;
    public GameObject aiCharacterPrefab;

    // TODO replace with an enum when relevant to do so
    public string gameMode = "party";
    public static string map = "game";

    void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        inputSprites.Init();
    }

    public static Sprite GetInputSprite(int index)
    {
        if (instance == null)
            return null;
        return instance.inputSprites.GetSpriteForInput(index);
    }
    public static Color GetColor(int index)
    {
        if(instance)
            return instance.playerColours[index];
        return Color.white;
    }


    public static void SetCharacterSelections(NodeElement[] selections)
    {
        //string res = "Character Selections made";
        //foreach (NodeElement e in selections)
        //    res += $" {e.ItemName},";
        //print(res);

        characterSelections = new Dictionary<int, GameObject>();
        for(int i = 0; i < selections.Length; i++)
            characterSelections.Add(i, selections[i].PrefabPayload);

        foreach (List<int> team in teams)
            if (team.Count == 0)
                characterSelections.Add(-1, instance.aiCharacterPrefab);
    }

    public static void SetRockSelections(NodeElement[] selections)
    {

    }


    [System.Serializable]
    public class TypedInputSprites
    {
        [Header("Input Sprites")]
        public Sprite joinSprite;
        public Sprite keySprite;
        public Sprite gamepadSprite;
        public Sprite bciSprite;
        public Sprite networkSprite;

        public Dictionary<InputType, Sprite> inputSprites;

        public void Init()
        {
            inputSprites = new Dictionary<InputType, Sprite>();

            inputSprites[InputType.invalid] = joinSprite;
            inputSprites[InputType.key] = keySprite;
            inputSprites[InputType.gamepad] = gamepadSprite;
            inputSprites[InputType.bci] = bciSprite;
            inputSprites[InputType.network] = networkSprite;
        }

        public Sprite GetSpriteForInput(int index)
        {
            return inputSprites[InputProxy.GetInputInfo(index).type];
        }
    }

    [System.Serializable]
    public class PlayerColours
    {
        [Header("Player Colours")]
        public Color[] colours;

        public Color this[int index]
        {
            get
            {
                if (index >= colours.Length || index < 0)
                    return Color.white;
                return colours[index];
            }
            set
            {
                colours[index] = value;
            }
        }
    }
}
