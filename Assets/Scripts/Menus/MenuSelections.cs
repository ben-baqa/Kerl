using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSelections : MonoBehaviour
{
    public static MenuSelections instance;
    public static List<List<int>> teams;
    public static List<GameObject> characterSelections;
    public static List<List<GameObject>> rockSelections;


    public TypedInputSprites inputSprites;

    // TODO replace with an enum when relevant to do so
    public string gameMode = "party";

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        instance = this;
        inputSprites.Init();
    }

    public static Sprite GetInputSprite(int index) => instance.inputSprites.GetSpriteForInput(index);

    public static void SetCharacterSelections(GridManager grid)
    {

    }

    public static void SetRockSelections(GridManager grid)
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
}
