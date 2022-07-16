using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CharacterManager : MonoBehaviour
{
    public GameObject fallbackCharacter;

    public Material blueTeam, redTeam;

    public float throwSpeedMultiplier = 2;

    [Header("Broom Defaults")]
    public Mesh broomHeadDefaultMesh;
    public Material[] broomHeadDefaultMaterials;
    public Mesh broomstickDefaultMesh;
    public Material[] broomstickDefaultMaterials;

    CharacterSet characterSet;

    Sweeper sweeper;

    Character throwerCharacter;
    Character sweeperCharacter;

    void Start()
    {
        List<GameObject> selectedCharacters = MenuSelections.characterSelections;
        if(selectedCharacters == null)
            selectedCharacters = new List<GameObject> { fallbackCharacter,
                fallbackCharacter, fallbackCharacter};

        List<List<int>> teams = MenuSelections.teams;
        if (teams == null)
            teams = MenuSelections.debugTeams;

        characterSet = new CharacterSet(selectedCharacters, teams);
        characterSet.SetTeamMaterials(blueTeam, redTeam, teams);
        characterSet.SetBrushMeshes(broomHeadDefaultMesh, broomHeadDefaultMaterials,
            broomstickDefaultMesh, broomstickDefaultMaterials);

        sweeper = FindObjectOfType<Sweeper>();

        ApplyIntroPlacements(FindObjectOfType<TeamIntro>().GetPlayerPlacments());
    }

    public void ApplyIntroPlacements(Placement[] characterPlacements)
    {
        Character[] characters = characterSet.GetCharacters();
        for(int i = 0; i < characters.Length; i++)
        {
            (characterPlacements[i] + characters[i].introPlacement).Apply(characters[i].transform);
            if (characters[i].hideBroomOnIntro)
                characters[i].HideBroom();
            characters[i].OnTeamIntro();
        }
    }

    public void OnTurnStart(int throwerIndex, int sweeperIndex)
    {
        characterSet.HideAll();

        throwerCharacter = characterSet[throwerIndex];
        throwerCharacter.SetUpThrowing();

        sweeperCharacter = characterSet[sweeperIndex];
        sweeperCharacter.SetUpBrushing();
        sweeper.SetCharacter(sweeperCharacter);
    }

    public void OnThrow()
    {
        throwerCharacter.OnThrow(throwSpeedMultiplier / sweeper.RockTravelTime);
        sweeperCharacter.OnThrow();
    }

    public Character GetCharacter(int playerIndex) => characterSet[playerIndex];

    class CharacterSet
    {
        List<IndexedCharacter> characters;
        int lastFetched;

        public CharacterSet(List<GameObject> selectedCharacters, List<List<int>> teams)
        {
            characters = new List<IndexedCharacter>();

            for(int i = 0; i < selectedCharacters.Count; i++)
            {
                characters.Add(InstantiateCharacter(selectedCharacters[i], i));
                if (IsPlayerOnSoloTeam(i, teams) || IsAITeam(i, teams))
                    characters.Add(InstantiateCharacter(selectedCharacters[i], i));
            }
        }

        IndexedCharacter InstantiateCharacter(GameObject target, int index)
        {
            GameObject characterObject = Instantiate(target);
            characterObject.name = $"P{index} character ({target.name})";
            Character instantiatedCharacter = characterObject.GetComponent<Character>();
            instantiatedCharacter.Init();

            return new IndexedCharacter(index, instantiatedCharacter);
        }

        private bool IsPlayerOnSoloTeam(int playerIndex, List<List<int>> teams)
        {
            foreach (List<int> team in teams)
                if (team.Count == 1 && team[0] == playerIndex)
                    return true;
            return false;
        }

        private bool IsAITeam(int playerIndex, List<List<int>> teams)
        {
            foreach (List<int> team in teams)
                if (team.Contains(playerIndex))
                    return false;
            return true;
        }

        public Character this[int index]
        {
            get
            {
                if (index < 0)
                    index = characters.Count - index;

                // search for an indexed character that was not fetched last time
                // this allows for one player index to use multiple characters
                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].playerIndex == index && i != lastFetched)
                    {
                        lastFetched = i;
                        return characters[i].character;
                    }
                }

                for (int i = 0; i < characters.Count; i++)
                {
                    if (characters[i].playerIndex == index)
                    {
                        lastFetched = i;
                        return characters[i].character;
                    }
                }

                print($"Character not found for index {index}");
                return null;
            }
        }

        public Character[] GetCharacters()
        {
            Character[] characterArray = new Character[characters.Count];
            for (int i = 0; i < characters.Count; i++)
                characterArray[i] = characters[i].character;

            return characterArray;
        }

        public void HideAll()
        {
            foreach (IndexedCharacter ic in characters)
                ic.character.Hide();
        }

        public void SetBrushMeshes(Mesh head, Material[] headMaterials, Mesh stick, Material[] stickMaterials)
        {
            foreach (IndexedCharacter ic in characters)
                ic.character.SetBrushMeshes(head, headMaterials, stick, stickMaterials);
        }

        public void SetTeamMaterials(Material team1Mat, Material team2Mat, List<List<int>> teams)
        {
            foreach (IndexedCharacter ic in characters)
            {
                if (teams[0].Contains(ic.playerIndex))
                    ic.character.SetTeamMaterial(team1Mat);
                if (teams[1].Contains(ic.playerIndex))
                    ic.character.SetTeamMaterial(team2Mat);
            }
        }

        struct IndexedCharacter
        {
            public int playerIndex;
            public Character character;

            public IndexedCharacter(int index, Character c)
            {
                playerIndex = index;
                character = c;
            }
        }
    }

    public class PlayerCharacter
    {
        public int playerIndex;
        public Character character;

        public PlayerCharacter(int index, Character c)
        {
            playerIndex = index;
            character = c;
        }

        public void Hide() => character.Hide();
        public void Init() => character.Init();
    }
}
